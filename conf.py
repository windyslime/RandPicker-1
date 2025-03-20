"""
配置文件和数据整理。
"""

import csv
import json
import pandas as pd


def get_with_short_id(num=1):
    """
    通过班级序号获取学生信息。

    :param num: 班级序号
    :return: 学生信息
    """
    with open('./students.json', 'r', encoding='utf-8') as f:
        students = json.load(f)
    for student in students['students']:
        if student['short_id'] == num:
            return student
    return None


def get_with_id(num=1):
    """
    通过学校全局学号获取学生信息。

    :param num: 全局学号
    :return: 学生信息
    """
    with open('./students.json', 'r', encoding='utf-8') as f:
        students = json.load(f)
    for student in students['students']:
        if student['id'] == num:
            return student
    return None


def get_students_num():
    """
    获取学生人数。

    :return: 学生人数
    """
    with open('./students.json', 'r', encoding='utf-8') as f:
        students = json.load(f)
    return len(students['students'])


def export2csv():  # WIP
    with open('./students.json', 'r', encoding='utf-8') as json_file:
        students = json.load(json_file)
    with open('./students.csv', 'w', newline='', encoding='utf-8') as csv_file:
        csv_writer = csv.writer(csv_file)


def excel2json(excel_path='./example.xlsx'):
    """
    从 Excel 文件 (.xls, .xlsx) 导入。

    注意：第 1 2 3 列必须分别为 班级序号 姓名 全局学号 。

    :param excel_path: Excel 文件路径
    :return:
    """
    sheet = pd.read_excel(excel_path)
    students = {}
    list_ = []
    for i in sheet.index.values:
        line = sheet.loc[i, ['short_id', 'name', 'id']].to_dict()
        list_.append(line)
    students['students'] = list_


def write_conf(students=None):
    """
    写入学生信息。

    :param students:
    :return:
    """
    if students is None:
        students = {}
    with open('./students.json', 'w', encoding='utf-8') as f:
        json.dump(students, f, ensure_ascii=False, indent=4, encoding='utf-8')
