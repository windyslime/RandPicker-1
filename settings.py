"""
设置。
"""

# 饼 做不做不一定
# 好好好在做了

from loguru import logger

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
