"""
设置。
"""

# 饼 做不做不一定
# 好好好在做了

from loguru import logger
from PyQt6 import uic
from qfluentwidgets import FluentWindow, FluentIcon as fIcon

settings = None


def open_settings():
    global settings
    if settings is None or not settings.isVisible():
        settings = Settings()
        settings.closed.connect(cleanup_settings)
        settings.show()
        logger.info('打开“设置”')
    else:
        settings.raise_()
        settings.activateWindow()

def cleanup_settings():
    global settings
    logger.info('关闭“设置”')
    del settings
    settings = None

class Settings(FluentWindow):
    def __init__(self):
        super().__init__()
        self.aboutInterface = uic.loadUi('./ui/settings/about.ui')
        self.aboutInterface.setObjectName('aboutInterface')

    def init_nav(self):
        self.addSubInterface(self.aboutInterface, fIcon.INFO, '关于')
    def setup_ui(self):
        self.setup_about_interface()

    def setup_about_interface(self):
        self.findChild(

        )
