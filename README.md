# Survivalcraft 汉化计划 - 字体

本分支包含位图字体(Bitmap Font)生成相关内容。仅限 Windows 操作系统使用
This branch contains content related to the generation of bitmap fonts (Bitmap Font). Only for Windows OS.

## 工作步骤:
1. 下载或克隆本项目
1. 安装 [Python3](https://www.python.org/downloads/) 用于执行转换脚本
1. 安装本项目根目录的`MicrosoftYaHeiPericles-Regular.ttf`字体（使用其他字体则不需要）
1. 准备需要的字符，以 `UTF-16` 编码存储到 `fonts\chars.txt` 中，注意不要有回车。目前 `chars.txt` 中包含了1.1万个常用简繁体汉字、各语言字母、数字、特殊符号
1. 可选：调整 `fonts\Pericles.bmfc` 中的设置，其中字体设置也在此处，需确保所有字符能输出到一张图片上（可以使用 `BMFont\bmfont64.exe` 加载该文件后在该软件中调整，再导出设置）
1. 可选：调整 `fonts\kerning.txt` 中的字符间距设置，第一行的数字是本文件有多少对字符间距设置，之后每行的第一个参数是前符号，第二个参数是后符号，第三个参数是将两个符号拉近的像素数，三个参数之间以`	`（Tab）相隔
1. 在终端中运行命令 ```python generate.py```
1. 输出结果在 `output` 目录

## Workflow:
1. Download or clone this repository.
1. Instal [Python3](https://www.python.org/downloads/) to execute scripts.
1. Install the `MicrosoftYaHeiPericles-Regular.ttf` font located in the root directory of this project. (Unless you use other font)
1. Prepare the required characters and store them with `UTF-16` encoding in `fonts\chars.txt`. No `\n`(carriage return) symbol in it. Currently, `chars.txt` includes 11 thousands commonly used simplified and traditional Chinese characters, letters from many languages, numbers, and special symbols.
1. Optional: Adjust the settings in `fonts\Pericles.bmfc`. The font name is in it. Outputting all characters on one texture is needed. (You can load this file by `BMFont\bmfont64.exe`, make adjustments within it, and then export the settings).
1. Optional: Adjust the character kerning settings in `fonts\kerning.txt`. The number on the first line indicates how many pairs of character kerning settings are in this file. Each subsequent line's first parameter is the preceding character, the second parameter is the following character, and the third parameter is the number of pixels to bring the two characters closer together. These three parameters are separated by a `	` (Tab).
1. Run the command ```python generate.py``` in the terminal.
1. The output results will be in the `output` directory.

## 说明
* 如果你调整了 `fonts\Pericles.bmfc` 中的 `fontSize`，你还需要调整 `generate.py` 第56行的缩放参数（默认值：`0.5`）
* 如果你的字体中没有 `▯`(U+25AF) 符号，你还需要调整 `generate.py` 第56行的后背符号参数（默认值：`▯`），或者给你的字体添加该默认符号

## Notes
* If you have adjusted the `fontSize` in `fonts\Pericles.bmfc`, you need to adjust the scaling parameter on line 56 of `generate.py` (default value: `0.5`).
* If your font does not contain the `▯` (U+25AF) symbol, you need to adjust the fallback character parameter on line 56 of `generate.py` (default value: `▯`), or add the default symbol to your font.

## 鸣谢 Thanks:
* [Bitmap Font Generator](http://www.angelcode.com/products/bmfont/)
* [0-v-0](https://github.com/0-v-0/SC-Chinese)