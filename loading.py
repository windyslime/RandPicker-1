"""
RandPicker 启动加载界面。
"""

import sys
from typing import Optional
from PyQt6.QtCore import Qt, QTimer, pyqtSignal, QPoint, QRect
from PyQt6.QtGui import QFont, QIcon, QPixmap, QPainter, QColor, QPainterPath, QBrush, QPen, QLinearGradient
from PyQt6.QtWidgets import (
    QApplication, QWidget, QVBoxLayout, QHBoxLayout,
    QLabel, QProgressBar, QSplashScreen
)
from qfluentwidgets import ProgressBar as FluentProgressBar, BodyLabel, CaptionLabel


class LoadingSplash(QSplashScreen):
    """启动加载界面"""
    
    progress_updated = pyqtSignal(int, str)  # 进度值和消息
    
    def __init__(self):
        super().__init__()
        self.setFixedSize(400, 350)
        self.setWindowIcon(QIcon("./img/Logo.png"))
        self.setWindowTitle("RandPicker 启动中...")
        
        # 设置无边框和置顶
        self.setWindowFlags(
            Qt.WindowType.SplashScreen | 
            Qt.WindowType.FramelessWindowHint |
            Qt.WindowType.WindowStaysOnTopHint
        )
    
        # 设置亚克力风格
        self.setAttribute(Qt.WidgetAttribute.WA_TranslucentBackground)
        self.setStyleSheet("""
            QSplashScreen {
                border-radius: 20px;
                background-color: rgba(255, 255, 255, 0.15);
                border: 1px solid rgba(255, 255, 255, 0.2);
            }
            
            QSplashScreen > QWidget {
                background: transparent;
            }
        """)
    
        self.init_ui()
        self.center_on_screen()
        
    def init_ui(self):
        """初始化界面"""
        # 主布局
        main_layout = QVBoxLayout()
        main_layout.setContentsMargins(30, 30, 30, 30)
        
        # Logo图片
        self.logo_label = QLabel()
        pixmap = QPixmap("/Users/jerrywu/RandPicker/img/Logo.png")
        scaled_pixmap = pixmap.scaledToHeight(80, Qt.TransformationMode.SmoothTransformation)
        self.logo_label.setPixmap(scaled_pixmap)
        self.logo_label.setAlignment(Qt.AlignmentFlag.AlignCenter)

        # 添加Logo到主布局
        main_layout.addWidget(self.logo_label)
        main_layout.addSpacing(15)

        # Logo和标题
        self.title_label = BodyLabel("RandPicker")
        self.title_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        title_font = QFont()
        title_font.setPointSize(20)
        title_font.setBold(True)
        self.title_label.setFont(title_font)
        self.title_label.setStyleSheet("color: white; font-weight: 600;")
        
        # 副标题
        self.subtitle_label = CaptionLabel("智能随机点名工具")
        self.subtitle_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.subtitle_label.setStyleSheet("color: rgba(255, 255, 255, 0.8);")
        
        # 状态文本
        self.status_label = CaptionLabel("正在初始化...")
        self.status_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.status_label.setStyleSheet("color: rgba(255, 255, 255, 0.9);")
        
        # 进度条
        self.progress_bar = FluentProgressBar()
        self.progress_bar.setFixedHeight(6)
        self.progress_bar.setRange(0, 100)
        self.progress_bar.setStyleSheet("""
            QProgressBar {
                background-color: rgba(255, 255, 255, 0.1);
                border-radius: 3px;
                border: none;
            }
            QProgressBar::chunk {
                background-color: rgba(255, 255, 255, 0.8);
                border-radius: 3px;
            }
        """)
        
        # 进度百分比
        self.percent_label = CaptionLabel("0%")
        self.percent_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.percent_label.setStyleSheet("color: rgba(255, 255, 255, 0.9);")
        
        # 错误状态组件（初始隐藏）
        self.error_icon = BodyLabel("⚠️")
        self.error_icon.setAlignment(Qt.AlignmentFlag.AlignCenter)
        error_icon_font = QFont()
        error_icon_font.setPointSize(24)
        self.error_icon.setFont(error_icon_font)
        self.error_icon.setStyleSheet("color: rgba(255, 107, 107, 0.9);")
        self.error_icon.hide()
        
        self.error_title = BodyLabel("启动失败")
        self.error_title.setAlignment(Qt.AlignmentFlag.AlignCenter)
        error_title_font = QFont()
        error_title_font.setPointSize(16)
        error_title_font.setBold(True)
        self.error_title.setFont(error_title_font)
        self.error_title.setStyleSheet("color: white; font-weight: 600;")
        self.error_title.hide()
        
        self.error_message = CaptionLabel("")
        self.error_message.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.error_message.setWordWrap(True)
        self.error_message.setStyleSheet("color: rgba(255, 255, 255, 0.9);")
        self.error_message.hide()
        
        self.error_details = CaptionLabel("")
        self.error_details.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.error_details.setWordWrap(True)
        self.error_details.setStyleSheet("color: rgba(255, 255, 255, 0.6);")
        self.error_details.hide()
        
        # 重试按钮
        from qfluentwidgets import PrimaryPushButton
        self.retry_button = PrimaryPushButton("重试")
        self.retry_button.setFixedWidth(120)
        self.retry_button.clicked.connect(self.retry_launch)
        self.retry_button.setStyleSheet("""
            PrimaryPushButton {
                background-color: rgba(255, 255, 255, 0.2);
                border: 1px solid rgba(255, 255, 255, 0.3);
                border-radius: 8px;
                color: white;
                font-weight: 500;
            }
            PrimaryPushButton:hover {
                background-color: rgba(255, 255, 255, 0.3);
                border: 1px solid rgba(255, 255, 255, 0.4);
            }
            PrimaryPushButton:pressed {
                background-color: rgba(255, 255, 255, 0.1);
            }
        """)
        self.retry_button.hide()
        
        # 按钮容器
        button_layout = QHBoxLayout()
        button_layout.addStretch()
        button_layout.addWidget(self.retry_button)
        button_layout.addStretch()
        
        # 添加到布局
        main_layout.addStretch()
        main_layout.addWidget(self.title_label)
        main_layout.addWidget(self.subtitle_label)
        main_layout.addSpacing(20)
        main_layout.addWidget(self.status_label)
        main_layout.addSpacing(10)
        main_layout.addWidget(self.progress_bar)
        main_layout.addWidget(self.percent_label)
        
        # 错误状态布局
        main_layout.addWidget(self.error_icon)
        main_layout.addWidget(self.error_title)
        main_layout.addWidget(self.error_message)
        main_layout.addWidget(self.error_details)
        main_layout.addLayout(button_layout)
        
        main_layout.addStretch()
        
        self.setLayout(main_layout)
        
        # 连接信号
        self.progress_updated.connect(self.update_progress)
        
    def paintEvent(self, event):
        """自定义绘制亚克力效果背景"""
        from PyQt6.QtCore import QRectF
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        
        # 创建圆角矩形路径
        path = QPainterPath()
        rect = QRectF(self.rect().adjusted(1, 1, -1, -1))
        path.addRoundedRect(rect, 20, 20)
        
        # 背景渐变
        gradient = QLinearGradient(0, 0, 0, self.height())
        gradient.setColorAt(0, QColor(255, 255, 255, 20))
        gradient.setColorAt(0.5, QColor(255, 255, 255, 15))
        gradient.setColorAt(1, QColor(255, 255, 255, 10))
        
        # 绘制背景
        painter.setClipPath(path)
        painter.fillPath(path, QBrush(gradient))
        
        # 绘制边框
        painter.setPen(QPen(QColor(255, 255, 255, 40), 1))
        painter.drawPath(path)
        
        # 添加内发光效果
        inner_glow = QPainterPath()
        inner_rect = QRectF(self.rect().adjusted(2, 2, -2, -2))
        inner_glow.addRoundedRect(inner_rect, 18, 18)
        painter.setPen(QPen(QColor(255, 255, 255, 60), 1))
        painter.drawPath(inner_glow)
        
    def center_on_screen(self):
        """居中显示"""
        from PyQt6.QtGui import QScreen
        screen = QApplication.primaryScreen()
        screen_geometry = screen.geometry()
        x = (screen_geometry.width() - self.width()) // 2
        y = (screen_geometry.height() - self.height()) // 2
        self.move(x, y)
        
    def update_progress(self, value: int, message: str):
        """更新进度"""
        self.progress_bar.setValue(value)
        self.status_label.setText(message)
        self.percent_label.setText(f"{value}%")
        
        # 强制刷新界面
        QApplication.processEvents()
        
    def show_error_state(self, error_message: str, details: str = ""):
        """显示错误状态"""
        # 隐藏进度相关组件
        self.progress_bar.hide()
        self.percent_label.hide()
        self.status_label.hide()
        
        # 显示错误相关组件
        self.error_icon.show()
        self.error_title.show()
        self.error_message.setText(error_message)
        self.error_message.show()
        
        if details:
            self.error_details.setText(details)
            self.error_details.show()
        else:
            self.error_details.hide()
            
        self.retry_button.show()
        
        # 强制刷新界面
        QApplication.processEvents()
        
    def retry_launch(self):
        """重试启动"""
        # 隐藏错误状态
        self.error_icon.hide()
        self.error_title.hide()
        self.error_message.hide()
        self.error_details.hide()
        self.retry_button.hide()
        
        # 重新显示进度状态
        self.progress_bar.show()
        self.percent_label.show()
        self.status_label.show()
        
        # 重置进度
        self.progress_bar.setValue(0)
        self.status_label.setText("正在重试...")
        self.percent_label.setText("0%")
        
        # 强制刷新界面
        QApplication.processEvents()
        
        # 通知加载管理器重试
        if hasattr(self.parent(), 'retry_loading'):
            self.parent().retry_loading()
        else:
            # 如果没有父窗口，直接重启应用
            from PyQt6.QtCore import QProcess
            QProcess.startDetached(sys.executable, ["main.py"])
            QApplication.quit()
        
    def closeEvent(self, event):
        """关闭事件"""
        event.accept()


class LoadingManager:
    """加载管理器"""
    
    def __init__(self):
        self.splash = None
        self.current_step = 0
        self.total_steps = 8  # 总加载步骤
        
    def init_splash(self):
        """初始化启动界面"""
        self.splash = LoadingSplash()
        self.splash.show()
        
    def update_progress(self, step: int, message: str):
        """更新加载进度"""
        if self.splash:
            progress = int((step / self.total_steps) * 100)
            self.splash.progress_updated.emit(progress, message)
            
    def show_error(self, error_message: str, details: str = ""):
        """显示错误信息"""
        if self.splash:
            # 更新界面为错误状态
            self.splash.show_error_state(error_message, details)
            
    def finish(self):
        """完成加载"""
        if self.splash:
            self.splash.close()
            self.splash = None


# 全局加载管理器实例
loading_manager = LoadingManager()