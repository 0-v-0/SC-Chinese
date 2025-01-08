#!/usr/bin/env python3
# Windows 平台下应该会根据后缀名判断文件类型

from fnt2lst import fnt2lst
import os
import subprocess
import warnings


input_dir = 'fonts'
output_dir = 'output'
bmfont_path = 'BMFont' + os.sep + 'bmfont64.exe'
cwebp_path = 'libwebp' + os.sep + 'cwebp.exe'
char_file_path = input_dir + os.sep + 'chars.txt'
kerning_file_path = input_dir + os.sep + 'kerning.txt'


def main():
    filelist = os.listdir(input_dir)
    filelist = [os.path.splitext(filename) for filename in filelist]
    bmfclist = [root for root, ext in filelist if ext == '.bmfc']
    if not os.path.exists(output_dir):
        os.mkdir(output_dir)
    for fontname in bmfclist:
        print('开始生成位图字体 Starting Generating Bitmap Font', fontname, '...')
        outputname_base = output_dir + os.sep + fontname
        cmdline = [bmfont_path]
        cmdline += ['-c', input_dir + os.sep + fontname + '.bmfc']
        cmdline += ['-o', outputname_base + '.fnt']
        cmdline += ['-t', char_file_path]
        print('调用 Calling bmfont64.exe ...')
        with subprocess.Popen(cmdline) as proc:
            pass
        if os.path.exists(outputname_base + '_1.png'):
            warnings.warn(fontname + ': 无法将所有字符输出到一张图片上 Unable to output all characters on one texture')
            os.remove(outputname_base + '_1.png')
            return
        if os.path.exists(outputname_base + '.webp'):
            os.remove(outputname_base + '.webp')
        cmdline2 = [cwebp_path]
        cmdline2 += ['-o', outputname_base + '.webp']
        cmdline2 += ['-preset', 'default']
        cmdline2 += ['-lossless']
        cmdline2 += ['-q', '100']
        cmdline2 += ['-z',  '9']
        cmdline2 += ['-m', '6']
        cmdline2 += ['-mt']
        cmdline2 += ['-exact']
        cmdline2 += [outputname_base + '_0.png']
        print('调用 Calling cwebp.exe ...')
        with subprocess.Popen(cmdline2) as proc:
            pass


        print('转换格式 Converting Format ...')
        fnt2lst(outputname_base + '.fnt', outputname_base + '.lst', kerning_file_path, 0.5, '▯') #▯:U+25AF
        print('清理缓存 Removing Caches ...')
        os.remove(outputname_base + '.fnt')
        os.remove(outputname_base + '_0.png')
        print('全部完成 All Done', fontname, '\n')
    
if __name__ == '__main__':
    main()