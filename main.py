"""
RandPicker 主程序。
"""

import os
import random
import sys
from random import choices
from typing import override
from datetime import datetime

from PyQt6 import uic
from PyQt6.QtCore import Qt, QPoint, QPropertyAnimation, QEasingCurve, QLocale
from PyQt6.QtGui import (
    QColor,
    QMouseEvent,
    QIcon,
    QPixmap,
    QPainter,
    QPainterPath,
    QPixmapCache,
)
from PyQt6.QtWidgets import (
    QApplication,
    QWidget,
    QLabel,
    QGraphicsDropShadowEffect,
    QSystemTrayIcon,
    QFrame,
    QLayout,
)
from loguru import logger
from qfluentwidgets import (
    PushButton,
    SystemTrayMenu,
    FluentIcon as fIcon,
    Action,
    Dialog,
    PrimaryPushButton,
    isDarkTheme,
    setTheme,
    Theme,
    qconfig,
    PixmapLabel,
    FluentTranslator,
    setThemeColor,
    SystemThemeListener,
)

import conf
from settings import open_settings, share, restart, update_history

# 适配高DPI缩放
QApplication.setHighDpiScaleFactorRoundingPolicy(
    Qt.HighDpiScaleFactorRoundingPolicy.PassThrough
)

# 建立几个空的全局变量
widget = None
last_result = {}
last_pos = QPoint()

# 记录日志
logger.add(
    "./log/RandPicker_{time}.log",
    rotation="1 MB",
    encoding="utf-8",
    retention="1 minute",
    compression="zip",
)

# 自动切换主题
qconfig.themeChanged.connect(lambda: reload_widget())


class Widget(QWidget):
    """
    主浮窗。
    """

    def __init__(self):
        super().__init__()
        self.m_Position = None
        self.p_Position = None
        self.r_Position = None
        self.is_avatar = False
        self.animation = None
        self.student = last_result
        self.init_ui()
        self.setWindowIcon(QIcon("./img/Logo.png"))
        self.systemTrayIcon = SystemTrayIcon(parent=self)
        self.systemTrayIcon.show()
        self.themeListener = SystemThemeListener(self)
        self.themeListener.start()

    def init_ui(self):
        global last_pos
        self.is_avatar = True if conf.ini.get("UI", "avatar") == "true" else False

        ui_file = f"./ui{'/dark/' if isDarkTheme() else '/'}{'widget.ui' if self.is_avatar else 'widget-no-avatar.ui'}"
        uic.loadUi(ui_file, self)

        logger.info(f"设置主题：{'深色' if isDarkTheme() else '浅色'}")

        # 设置窗口无边框和透明背景
        self.setAttribute(Qt.WidgetAttribute.WA_TranslucentBackground)

        if sys.platform == "darwin":
            self.setWindowFlags(
                Qt.WindowType.FramelessWindowHint
                | Qt.WindowType.WindowStaysOnTopHint
                | Qt.WindowType.Widget
            )
        else:
            self.setWindowFlags(
                Qt.WindowType.FramelessWindowHint
                | Qt.WindowType.WindowStaysOnTopHint
                | Qt.WindowType.Tool
            )

        self.layout().setSizeConstraint(QLayout.SizeConstraint.SetFixedSize)

        if last_pos:
            logger.info(f"移动到重载前的位置 ({last_pos.x()}, {last_pos.y()})。")
            self.move(last_pos)
        elif conf.ini.get("Last", "x") and conf.ini.get("Last", "y"):
            x = int(conf.ini.get("Last", "x"))
            y = int(conf.ini.get("Last", "y"))
            pos = QPoint(x, y)
            logger.info(f"移动到上次关闭的位置 ({x}, {y})。")
            self.move(pos)

        background = self.findChild(QFrame, "backgnd")
        shadow_effect = QGraphicsDropShadowEffect(self)
        shadow_effect.setBlurRadius(28)
        shadow_effect.setXOffset(0)
        shadow_effect.setYOffset(6)
        shadow_effect.setColor(QColor(0, 0, 0, 75))
        background.setGraphicsEffect(shadow_effect)

        btn_person = self.findChild(PrimaryPushButton, "btn_person")
        btn_person.clicked.connect(lambda: self.pick_person())

        btn_group = self.findChild(PushButton, "btn_group")
        btn_group.clicked.connect(lambda: self.pick_group())

        self.clear()

    @override
    def mousePressEvent(self, event: QMouseEvent):
        edge_distance = int(conf.ini.get("UI", "edge_distance"))
        if event.button() == Qt.MouseButton.LeftButton:
            # 检查窗口是否处于隐藏状态
            screen = QApplication.screenAt(event.globalPosition().toPoint())
            if not screen:
                screen = QApplication.primaryScreen()
            screen_geometry = screen.geometry()
            window_geometry = self.geometry()

            # 如果窗口被隐藏在左边或右边，则恢复显示
            if (
                window_geometry.left() < screen_geometry.left()
                or window_geometry.right() > screen_geometry.right()
                or window_geometry.top() < screen_geometry.top()
                or window_geometry.bottom() > screen_geometry.bottom()
            ):
                # 计算窗口应该显示的位置
                if window_geometry.left() < screen_geometry.left():
                    window_geometry.moveLeft(screen_geometry.left() + edge_distance)
                else:
                    window_geometry.moveRight(screen_geometry.right() - edge_distance)
                self.setGeometry(window_geometry)
                logger.info("窗口恢复显示")
                event.accept()
                return

            # 如果窗口未隐藏，则处理正常的拖动
            self.m_Position = (
                event.globalPosition().toPoint() - self.pos()
            )  # 获取鼠标相对窗口的位置
            self.p_Position = event.globalPosition().toPoint()  # 获取鼠标相对屏幕的位置
            event.accept()

    @override
    def mouseMoveEvent(self, event: QMouseEvent):
        if not self.m_Position:
            return
        if event.buttons() == Qt.MouseButton.LeftButton:
            self.move(
                event.globalPosition().toPoint() - self.m_Position
            )  # 更改窗口位置
            self.r_Position = event.globalPosition().toPoint()  # 记录鼠标释放位置
            event.accept()

    def pick_person(self):
        """
        随机选人。
        """
        global historys
        students = []

        if (
            conf.ini.get("Group", "global") == "true"
            or conf.ini.get("Group", "group") == ""
        ):
            logger.debug("使用全局分组。")
            students = conf.stu.get_active_index()
        else:
            groups = conf.ini.get("Group", "groups").split(", ")
            logger.debug(f"使用分组 {groups}。")
            for group in groups:
                students.extend(conf.group.get_stu_index(int(group)))

        name = self.findChild(QLabel, "name")
        id_ = self.findChild(QLabel, "id")

        if not students:
            name.setText("无结果")
            id_.setText("000000")

        num = choices(students, weights=conf.stu.get_weight(students), k=1)[0]
        logger.info(
            f"随机数已生成。JSON 索引是 {num}。它的选择权重是 {conf.stu.get_all_weight()[num]}。"
        )
        self.student = conf.stu.get_single(num)
        logger.debug(f"已获取 JSON 索引是 {num} 的学生信息。{self.student}")
        name.setText(f"{str(self.student['id'])[-2:]} {self.student['name']}")
        id_.setText(str(self.student["id"]))

        if self.is_avatar:
            # 设置头像
            avatar_path = None
            # 尝试不同的图片格式
            for ext in ["png", "jpg", "jpeg"]:
                temp_path = f"./img/stu/{self.student['id']}.{ext}"
                if os.path.exists(temp_path):
                    avatar_path = temp_path
                    logger.success(
                        f"找到了学生 {self.student['id']} 的头像 {self.student['id']}.{ext}。"
                    )
                    break
            self.student["avatar"] = avatar_path
            self.show_avatar(avatar_path)

        conf.history.add_history(
            {
                "mode": 0,
                "student": self.student,
                "time": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            }
        )
        update_history()  # 通知历史记录已更改

    def clear(self):
        global last_result
        name = self.findChild(QLabel, "name")
        id_ = self.findChild(QLabel, "id")
        if last_result:
            name.setText(f"{str(self.student['id'])[-2:]} {last_result['name']}")
            id_.setText(str(last_result["id"]))
            if self.is_avatar:
                self.show_avatar(last_result["avatar"])
            last_result = {}
            logger.info(f"加载重载前的结果。{last_result}")
        else:
            name.setText("无结果")
            id_.setText("000000")
            if self.is_avatar:
                self.show_avatar()
            logger.info("清除结果")

    def pick_group(self):
        """
        随机选小组。

        """

        groups = len(conf.group.get_all())
        if groups < 1:
            self.pick_person()
            return
        num = random.randint(0, groups - 1)
        logger.debug(f"随机数已生成。小组的 JSON 索引是 {num}。")
        group = conf.group.get_single(num)
        logger.debug(f"已获取 JSON 索引是 {num} 的小组信息。{group}")
        student_names = conf.group.get_stu_name(group)

        students = ", ".join(student_names)

        name = self.findChild(QLabel, "name")
        id_ = self.findChild(QLabel, "id")
        logger.debug(f"信息已解析。名称：{group['name']}；学生：{students}。")

        name.setText(group["name"])
        id_.setText(students)
        if self.is_avatar:
            self.show_avatar()

        group["id"] = students

        conf.history.add_history(
            {
                "mode": 1,
                "student": group,
                "time": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            }
        )
        update_history()  # 通知历史记录已更改

    def show_avatar(self, file_path="./img/stu/default.jpeg"):
        avatar = self.findChild(PixmapLabel, "avatar")
        if not avatar:
            return
        avatar_size = int(conf.ini.get("UI", "avatar_size"))
        if file_path is not None and os.path.exists(file_path):
            file_path = file_path
        elif os.path.exists("./img/stu/default.jpeg"):
            logger.warning("没有找到头像。使用默认头像。")
            file_path = "./img/stu/default.jpeg"
        else:
            avatar.setPixmap(QPixmap())
            avatar.setStyleSheet(
                f"border-radius: {avatar_size // 2}px; background-color: transparent;"
            )
            logger.warning(f"没有找到头像 {file_path} 和默认头像。使用空白。")
            return

        # 使用复合缓存键，避免重复创建
        final_cache_key = f"final_{file_path}_{avatar_size}"
        final_pixmap = QPixmapCache.find(final_cache_key)

        if not final_pixmap:
            # 原始图片缓存
            cache_key = f"{file_path}_{avatar_size}"
            pixmap = QPixmapCache.find(cache_key)
            if not pixmap:
                pixmap = QPixmap(file_path)
                scaled_pixmap = pixmap.scaled(
                    avatar_size,
                    avatar_size,
                    Qt.AspectRatioMode.KeepAspectRatio,
                    Qt.TransformationMode.SmoothTransformation,
                )
                QPixmapCache.insert(cache_key, scaled_pixmap)
                pixmap = scaled_pixmap

            # 创建最终带圆形遮罩的图片
            final_pixmap = QPixmap(avatar_size, avatar_size)
            final_pixmap.fill(Qt.GlobalColor.transparent)

            with QPainter(final_pixmap) as painter:
                painter.setRenderHint(QPainter.RenderHint.Antialiasing)

                # 先设置圆形裁剪区域
                path = QPainterPath()
                path.addEllipse(0, 0, avatar_size, avatar_size)
                painter.setClipPath(path)

                # 然后在裁剪区域内绘制图片
                x = (avatar_size - pixmap.width()) // 2
                y = (avatar_size - pixmap.height()) // 2
                painter.drawPixmap(x, y, pixmap)

            QPixmapCache.insert(final_cache_key, final_pixmap)

        avatar.setPixmap(final_pixmap)
        logger.success(f"显示头像 {file_path}。")
        avatar.setStyleSheet(
            f"border-radius: {avatar_size // 2}px; background-color: transparent;"
        )

    @override
    def mouseReleaseEvent(self, event: QMouseEvent):
        screen = QApplication.screenAt(event.globalPosition().toPoint())
        if not screen:
            screen = QApplication.primaryScreen()
        screen_geometry = screen.geometry()
        edge_distance = int(conf.ini.get("UI", "edge_distance"))
        hidden_width = int(conf.ini.get("UI", "hidden_width"))
        window_geometry = self.geometry()
        if (
            event.button() == Qt.MouseButton.LeftButton
            and conf.ini.get("UI", "edge_hide") == "true"
            and self.r_Position is not None
        ):
            # 检测是否靠近屏幕边缘
            if window_geometry.left() < screen_geometry.left() + edge_distance:
                # 靠左边缘
                self.animation = QPropertyAnimation(self, b"geometry")
                elastic_enabled = conf.ini.get("UI", "elastic_animation") == "true"
                if elastic_enabled:
                    self.animation.setDuration(300)
                    self.animation.setEasingCurve(QEasingCurve.Type.OutBounce)
                else:
                    self.animation.setDuration(150)
                    self.animation.setEasingCurve(QEasingCurve.Type.Linear)
                logger.debug(
                    f"弹性动画状态: {elastic_enabled}, 持续时间: {self.animation.duration()}ms, 缓动曲线: {self.animation.easingCurve().type()}"
                )
                self.animation.setStartValue(window_geometry)
                window_geometry.moveRight(screen_geometry.left() + hidden_width)
                self.animation.setEndValue(window_geometry)
                self.animation.start()
                logger.info("窗口贴靠到左边缘")
            elif window_geometry.right() > screen_geometry.right() - edge_distance:
                # 靠右边缘
                self.animation = QPropertyAnimation(self, b"geometry")
                elastic_enabled = conf.ini.get("UI", "elastic_animation") == "true"
                if elastic_enabled:
                    self.animation.setDuration(300)
                    self.animation.setEasingCurve(QEasingCurve.Type.OutBounce)
                else:
                    self.animation.setDuration(150)
                    self.animation.setEasingCurve(QEasingCurve.Type.Linear)
                logger.debug(
                    f"弹性动画状态: {elastic_enabled}, 持续时间: {self.animation.duration()}ms, 缓动曲线: {self.animation.easingCurve().type()}"
                )
                self.animation.setStartValue(window_geometry)
                window_geometry.moveLeft(screen_geometry.right() - hidden_width)
                self.animation.setEndValue(window_geometry)
                self.animation.start()
                logger.info("窗口贴靠到右边缘。")
            elif window_geometry.top() < screen_geometry.top() + edge_distance:
                # 靠上边缘
                self.animation = QPropertyAnimation(self, b"geometry")
                elastic_enabled = conf.ini.get("UI", "elastic_animation") == "true"
                if elastic_enabled:
                    self.animation.setDuration(300)
                    self.animation.setEasingCurve(QEasingCurve.Type.OutBounce)
                else:
                    self.animation.setDuration(150)
                    self.animation.setEasingCurve(QEasingCurve.Type.Linear)
                logger.debug(
                    f"弹性动画状态: {elastic_enabled}, 持续时间: {self.animation.duration()}ms, 缓动曲线: {self.animation.easingCurve().type()}"
                )
                self.animation.setStartValue(window_geometry)
                window_geometry.moveTop(screen_geometry.top() + edge_distance)
                self.animation.setEndValue(window_geometry)
                self.animation.start()
                logger.info("窗口调整到屏幕顶部内。")
            elif window_geometry.bottom() > screen_geometry.bottom() - edge_distance:
                # 靠下边缘
                self.animation = QPropertyAnimation(self, b"geometry")
                elastic_enabled = conf.ini.get("UI", "elastic_animation") == "true"
                if elastic_enabled:
                    self.animation.setDuration(300)
                    self.animation.setEasingCurve(QEasingCurve.Type.OutBounce)
                else:
                    self.animation.setDuration(150)
                    self.animation.setEasingCurve(QEasingCurve.Type.Linear)
                logger.debug(
                    f"弹性动画状态: {elastic_enabled}, 持续时间: {self.animation.duration()}ms, 缓动曲线: {self.animation.easingCurve().type()}"
                )
                self.animation.setStartValue(window_geometry)
                window_geometry.moveBottom(screen_geometry.bottom() - edge_distance)
                self.animation.setEndValue(window_geometry)
                self.animation.start()
                logger.info("窗口调整到屏幕底部内。")

        event.accept()

    @override
    def closeEvent(self, e):
        global last_result, last_pos
        self.systemTrayIcon.hide()
        self.systemTrayIcon.deleteLater()
        last_result = self.student
        last_pos = self.pos()
        conf.ini.write("Last", "x", last_pos.x(), "Last", "y", last_pos.y())
        self.themeListener.terminate()
        self.themeListener.deleteLater()
        super().closeEvent(e)


def reload_widget():
    global widget
    if widget is None:
        return
    if widget.isVisible():
        widget.close()
    logger.debug("重载浮窗")
    init()


def init():
    global widget
    if isDarkTheme():
        setThemeColor(conf.ini.get("Color", "dark"))
    else:
        setThemeColor(conf.ini.get("Color", "light"))
    widget = Widget()
    widget.show()
    widget.raise_()


def stop():
    global widget
    if widget.isVisible():
        widget.close()
    sys.exit()


class SystemTrayIcon(QSystemTrayIcon):
    def __init__(self, parent):
        super().__init__(parent=parent)
        self.setIcon(parent.windowIcon())
        self.menu = SystemTrayMenu(title="RandPicker", parent=parent)
        self.menu.addActions(
            [
                Action(fIcon.SETTING, "设置", triggered=lambda: open_settings()),
            ]
        )
        self.menu.addSeparator()
        self.menu.addActions(
            [
                Action("重载", triggered=lambda: reload_widget()),
                Action(fIcon.SYNC, "重新启动", triggered=lambda: restart()),
            ]
        )
        self.menu.addSeparator()
        self.menu.addAction(Action(fIcon.CLOSE, "关闭", triggered=lambda: stop()))
        self.setContextMenu(self.menu)


if __name__ == "__main__":
    os.environ["QT_SCALE_FACTOR"] = str(float(conf.ini.get("General", "scale")))
    app = QApplication(sys.argv)
    translator = FluentTranslator(
        QLocale(QLocale.Language.Chinese, QLocale.Country.China)
    )
    app.installTranslator(translator)
    logger.info(f"RandPicker 启动。缩放系数 {os.environ['QT_SCALE_FACTOR']}。")
    if share.isAttached():
        logger.warning("有一个实例正在运行，或者上次没有正常退出。")
        logger.error("不欢迎。")
        msg_box = Dialog(
            "RandPicker 正在运行",
            "RandPicker 正在运行！请勿打开多个实例，否则将会出现不可预知的问题。",
        )
        msg_box.yesButton.setText("好")
        msg_box.cancelButton.hide()
        msg_box.buttonLayout.insertStretch(0, 1)
        msg_box.setFixedWidth(550)
        msg_box.exec()
        logger.info("退出。")
        sys.exit(-1)
    share.create(1)
    logger.info("欢迎。")
    # 设置主题
    if conf.ini.get("General", "theme") == "0":
        setTheme(Theme.LIGHT)
    elif conf.ini.get("General", "theme") == "1":
        setTheme(Theme.DARK)
    else:
        setTheme(Theme.AUTO)
    init()

    app.setQuitOnLastWindowClosed(False)

    sys.exit(app.exec())
