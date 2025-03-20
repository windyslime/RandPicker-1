import csv
import json


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

def export2csv(): # WIP
    with open('./students.json', 'r', encoding='utf-8') as json_file:
        students = json.load(json_file)
    with open('./students.csv', 'w', newline='', encoding='utf-8') as csv_file:
        csv_writer = csv.writer(csv_file)

get_with_short_id()
