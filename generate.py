#!/usr/bin/env python3
# Windows 平台下应该会根据后缀名判断文件类型

from fnt2lst import fnt2lst
import os
import subprocess
import warnings


input_dir = 'fonts'
output_dir = 'output'
bmfont_path = 'BMFont' + os.sep + 'bmfont.exe'
char_file_path = input_dir + os.sep + 'chars.txt'


def main():
    filelist = os.listdir(input_dir)
    filelist = [os.path.splitext(filename) for filename in filelist]
    bmfclist = [root for root, ext in filelist if ext == '.bmfc']
    if not os.path.exists(output_dir):
        os.mkdir(output_dir)
    for fontname in bmfclist:
        print('开始生成字体', fontname, '...')
        outputname_base = output_dir + os.sep + fontname
        cmdline = [bmfont_path]
        cmdline += ['-c', input_dir + os.sep + fontname + '.bmfc']
        cmdline += ['-o', outputname_base + '.fnt']
        cmdline += ['-t', char_file_path]
        print('调用 bmfont.exe ...')
        with subprocess.Popen(cmdline) as proc:
            pass
        if os.path.exists(outputname_base + '_1.png'):
            warnings.warn(fontname + ': 请使用更大的输出纹理')
        if os.path.exists(outputname_base + '.png'):
            os.remove(outputname_base + '.png')
        os.rename(outputname_base + '_0.png', outputname_base + '.png')

        print('转换格式 ...')
        fnt2lst(outputname_base + '.fnt', outputname_base + '.lst')
        print('过河拆桥 ...')
        os.remove(outputname_base + '.fnt')
        print('字体生成完成', fontname, '\n')
    
if __name__ == '__main__':
    main()