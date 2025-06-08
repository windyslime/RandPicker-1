import json
import os
from . import ini

from loguru import logger

historys = []
record = ini.get("History", "record") == "true"


def _refresh_record():
    global record
    record = ini.get("History", "record") == "true"


def load_history():
    global historys
    try:
        with open("log/history.json", "r", encoding="utf-8") as f:
            historys = json.load(f)["historys"]
    except FileNotFoundError:
        historys = []
    except json.JSONDecodeError:
        historys = []


def add_history(history: dict):
    _refresh_record()
    global historys, record
    historys.insert(0, history)
    if record:
        save_history()


def save_history():
    global historys
    if os.path.exists("log/history.json"):
        with open("log/history.json", "r", encoding="utf-8") as f:
            try:
                data: dict = json.load(f)
            except json.JSONDecodeError:
                data = {"historys": []}
                logger.error("历史记录文件格式错误，已重置。")
    else:
        data = {"historys": []}

    data["historys"].insert(0, historys[0])

    with open("log/history.json", "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=4)
        logger.info(f"历史记录已保存。新增加了 {historys[0]}")


if record:
    load_history()
