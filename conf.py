import json
import csv

def get(num=1):
    with open('./students.json', 'r', encoding='utf-8') as f:
        students = json.load(f)
    for student in students['students']:
        if student['short_id'] == num:
            return student
    return None

def export2csv(): # WIP
    with open('./students.json', 'r', encoding='utf-8') as json_file:
        students = json.load(json_file)
    with open('./students.csv', 'w', newline='', encoding='utf-8') as csv_file:
        csv_writer = csv.writer(csv_file)
