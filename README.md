# Survivalcraft 汉化计划 - 字体

本分支包含 bitmap font 生成相关内容。

## 准备工作:
* [scpak](https://github.com/qnnnez/scpak): 用于打包字库
* [python3](https://www.python.org/downloads/): 用于执行转换脚本
* 安装 PericlesYH 字体

## 工作步骤:
1. 打开 Bitmap Font Generator，选择需要生成的字体，并设定字体尺寸等相关信息
1. 选择需要的汉字，放在 chars.txt (位于 fonts 目录) 中，使用 utf-16 编码。目前 chars.txt 中包含了近 5000 个常用汉字、字母、数字、特殊符号
1. 调整输出选项，使字体能保存在一张纹理当中，并尽可能小 (注意，Survivalcraft 不能读取超过长或宽超过 2048 像素的纹理)
1. 将字体信息保存到 fonts 文件夹中
1. 执行 ```generate.py```
1. 将 output 目录中生成的字体复制到 Fonts 目录中
1. 打包 pak

## TODO List:
* 待文本翻译完成，更新 chars.txt，使字库刚好够用

## Thanks:
* [Bitmap Font Generator](http://www.angelcode.com/products/bmfont/)
