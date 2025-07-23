#!/usr/bin/env python3
"""
导出功能测试脚本
"""

import sys
import os
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from conf.export import ResultExporter, export_recent_results
from conf.history import historys
from conf.stu import add_student
from datetime import datetime
import json


def setup_test_data():
    """设置测试数据"""
    # 清空现有历史记录
    historys.clear()
    
    # 添加测试学生
    add_student("张三", 1001, True, 1.0)
    add_student("李四", 1002, True, 1.2)
    add_student("王五", 1003, True, 0.8)
    
    # 添加测试历史记录
    test_history = [
        {
            "mode": 0,
            "student": {"name": "张三", "id": 1001},
            "time": datetime.now(),
            "note": "个人抽选测试"
        },
        {
            "mode": 1,
            "student": {"name": "李四", "id": 1002},
            "time": datetime.now(),
            "note": "分组抽选测试"
        },
        {
            "mode": 2,
            "student": {"name": "王五", "id": 1003},
            "time": datetime.now(),
            "note": "权重抽选测试"
        }
    ]
    
    historys.extend(test_history)
    
    # 保存测试数据
    with open("./conf/history.json", "w", encoding="utf-8") as f:
        json.dump(historys, f, ensure_ascii=False, indent=2, default=str)
    
    print("测试数据已准备完成")


def test_export():
    """测试导出功能"""
    exporter = ResultExporter()
    
    try:
        print("正在测试Excel导出...")
        excel_path = exporter.export_to_excel(historys)
        print(f"Excel导出成功: {excel_path}")
        
        print("正在测试CSV导出...")
        csv_path = exporter.export_to_csv(historys)
        print(f"CSV导出成功: {csv_path}")
        
        print("正在测试JSON导出...")
        json_path = exporter.export_to_json(historys)
        print(f"JSON导出成功: {json_path}")
        
        print("正在测试PDF导出...")
        pdf_path = exporter.export_to_pdf(historys)
        print(f"PDF导出成功: {pdf_path}")
        
        print("正在测试全部导出...")
        all_results = exporter.export_all_formats(historys)
        print(f"全部导出成功: {all_results}")
        
    except Exception as e:
        print(f"导出测试失败: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    print("RandPicker 导出功能测试")
    print("=" * 30)
    
    # 设置测试数据
    setup_test_data()
    
    # 运行测试
    test_export()
    
    print("\n测试完成！请检查 ./exports 目录下的导出文件")