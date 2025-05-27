import sys
import os

from PyQt6.QtCore import QThread, pyqtSignal
from PyQt6.QtWidgets import QApplication
from packaging.version import Version
import requests
from loguru import logger

import conf


if sys.platform == "win32":
    import win32api

REPO = "xuanxuan1231/RandPicker"
UPDATER_REPO = "xuanxuan1231/RandPicker-Updater"
APP_VERSION = Version("v1.6.0")

if sys.platform == "win32":
    # 获取更新器的版本
    try:
        info = win32api.GetFileVersionInfo("./Updater.exe", "\\")
        ms = info["FileVersionMS"]
        ls = info["FileVersionLS"]
        UPDATER_VERSION = Version(
            f"{win32api.HIWORD(ms)}.{win32api.LOWORD(ms)}.{win32api.HIWORD(ls)}.{win32api.LOWORD(ls)}"
        )
    except Exception as e:
        logger.error(f"获取更新器版本时发生错误: {e}")
        UPDATER_VERSION = Version("0.0.0")


def check_update_app(origin: int = 0) -> dict:
    """
    检查更新并返回最新版本信息。

    :param origin: 更新源。
                   '1' 表示使用 OSS 源，'0' 表示使用 GitHub 源。
                   默认值为 '0'。
    :type origin: int
    :return: 包含最新版本号和是否是最新版本的字典
    """

    logger.info("开始检查更新。")
    result = {}

    if origin == 1:
        MANIFEST_URL = "https://oss.may.pp.ua/RandPicker/latest.json"
    else:
        MANIFEST_URL = f"https://api.github.com/repos/{REPO}/releases/latest"

    try:
        response = requests.get(MANIFEST_URL, timeout=5)
        response.raise_for_status()
        latest_version = Version(response.json().get("tag_name"))

    except requests.RequestException as e:
        logger.error(f"检查更新时发生错误。{e}")
        return None

    if latest_version > APP_VERSION:
        is_latest = False
        logger.info(f"发现新版本 {latest_version}。")
    else:
        is_latest = True
        logger.info("当前版本是最新的。")

    result = {"version": str(latest_version), "is_latest": is_latest}

    return result


def check_update_updater(origin: int = 0) -> dict:
    """
    检查更新器的更新并返回最新版本信息。

    :return: 更新器的版本号
    """
    logger.info("开始检查更新器更新。")
    result = {}

    if origin == 1:
        MANIFEST_URL = "https://oss.may.pp.ua/Updater/latest.json"
    else:
        MANIFEST_URL = f"https://api.github.com/repos/{UPDATER_REPO}/releases/latest"

    try:
        response = requests.get(MANIFEST_URL, timeout=5)
        response.raise_for_status()
        latest_version = Version(response.json().get("tag_name"))

    except requests.RequestException as e:
        logger.error(f"检查更新时发生错误。{e}")
        return None

    if latest_version > UPDATER_VERSION:
        is_latest = False
        logger.info(f"发现新版本 {latest_version}。")
    else:
        is_latest = True
        logger.info("当前版本是最新的。")

    if origin == "oss":
        result = {
            "version": str(latest_version),
            "url": f"https://oss.may.pp.ua/Updater/{latest_version}/Updater.zip",
            "is_latest": is_latest,
        }
    else:
        try:
            result = {
                "version": str(latest_version),
                "url": response.json().get("assets")[0].get("browser_download_url"),
                "is_latest": is_latest,
            }
        except IndexError:
            logger.error("无法获取更新器的下载链接。")
            result = {
                "version": str(latest_version),
                "url": None,
                "is_latest": is_latest,
            }

    return result


def update_app(
    parent=None,
):
    """
    更新应用程序。
    """
    logger.info("开始更新应用程序。")
    origin = conf.ini.get("Update", "app")
    if os.path.exists("Updater.exe"):
        os.system(f"start Updater.exe -l=false -origin={origin}")
        QApplication.quit()
    logger.error("未找到更新器。")


def refresh_updater_version(parent=None):
    """
    更新更新器版本。
    """
    global UPDATER_VERSION
    if sys.platform == "win32":
        # 获取更新器的版本
        try:
            info = win32api.GetFileVersionInfo("./Updater.exe", "\\")
            ms = info["FileVersionMS"]
            ls = info["FileVersionLS"]
            UPDATER_VERSION = Version(
                f"{win32api.HIWORD(ms)}.{win32api.LOWORD(ms)}.{win32api.HIWORD(ls)}.{win32api.LOWORD(ls)}"
            )
        except Exception as e:
            logger.error(f"获取更新器版本时发生错误: {e}")
            UPDATER_VERSION = Version("0.0.0")
    return


class UpdateUpdaterThread(QThread):
    progressChanged = pyqtSignal(int)
    finished = pyqtSignal(bool, str)

    def __init__(self, url, parent=None):
        super().__init__(parent)
        self.url = url
        self.error_msg = ""

    def run(self):
        BACKUP_URLS = [
            f"https://ghfast.top/{self.url}",
            f"https://gh-proxy.com/{self.url}",
            f"https://github.moeyy.xyz/{self.url}",
        ]
        try:
            self.progressChanged.emit(10)
            try:
                resp = requests.get(self.url, timeout=10, stream=True)
                total = int(resp.headers.get("content-length", 0))
                downloaded = 0
                chunk_size = 8192
                with open("Updater.exe", "wb") as f:
                    for chunk in resp.iter_content(chunk_size=chunk_size):
                        if chunk:
                            f.write(chunk)
                            downloaded += len(chunk)
                            if total > 0:
                                percent = int(10 + 90 * downloaded / total)
                                self.progressChanged.emit(percent)
                self.progressChanged.emit(100)
                self.finished.emit(True, "")
                return
            except Exception as e:
                logger.error(f"主下载源失败: {e}")
                self.progressChanged.emit(15)
                if not self.url.startswith("https://github.com"):
                    self.error_msg = f"下载失败: {e}"
                    self.finished.emit(False, self.error_msg)
                    return
                # 依次尝试备用源
                for backup_url in BACKUP_URLS:
                    try:
                        resp = requests.get(backup_url, timeout=10, stream=True)
                        total = int(resp.headers.get("content-length", 0))
                        downloaded = 0
                        chunk_size = 8192
                        with open("Updater.exe", "wb") as f:
                            for chunk in resp.iter_content(chunk_size=chunk_size):
                                if chunk:
                                    f.write(chunk)
                                    downloaded += len(chunk)
                                    if total > 0:
                                        percent = int(10 + 90 * downloaded / total)
                                        self.progressChanged.emit(percent)
                        self.progressChanged.emit(100)
                        self.finished.emit(True, "")
                        return
                    except Exception as e2:
                        logger.error(f"备用下载源 {backup_url} 失败: {e2}")
                        self.progressChanged.emit(15)
                # 全部失败
                self.error_msg = f"所有下载源均失败: {e}"
                self.finished.emit(False, self.error_msg)
        except Exception as e:
            self.error_msg = str(e)
            self.finished.emit(False, self.error_msg)


"""if __name__ == '__main__':
    print(check_for_update('github'))"""
