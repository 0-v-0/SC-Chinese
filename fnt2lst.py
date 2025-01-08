#!/usr/bin/env python3
# Windows 平台下应该会根据后缀名判断文件类型

import os

class Line(dict):
    def __init__(self, line):
        super().__init__(self)
        line = line.strip()
        comps = line.split(' ')
        if '=' not in comps[0]:
            self.name = comps[0]
            del comps[0]
        else:
            self.name = None
        for comp in comps:
            comp = comp.strip()
            if not comp:
                continue
            key, value = comp.split('=')
            self[key] = value

def fnt2lst(fnt_path, lst_path, kerning_path, scale=1, fallback='_'):
    lst_file = open(lst_path, 'w', encoding='utf-8')
    lines = open(fnt_path, 'r', encoding='utf-8').readlines()

    infos = Line(lines[0][lines[0].find("spacing="):])
    spacing = infos['spacing'].split(',')

    commons = Line(lines[1])
    line_height = int(commons['lineHeight'])
    base = int(commons['base'])
    width = int(commons['scaleW'])
    height = int(commons['scaleH'])

    char_count = int(Line(lines[3])['count'])

    flag = True
    line1 = lines[4]
    infos1 = Line(line1)
    if int(infos1['id']) < 0:
        flag = False

    lst_file.write('{}\n'.format(char_count if flag else char_count - 1))

    for i in range(4, char_count + 4):
        line = lines[i]
        infos = Line(line)
        id = int(infos['id'])
        if id < 0:
            continue
        unicode = chr(id)
        x = int(infos['x'])
        y = int(infos['y'])
        w = int(infos['width'])
        h = int(infos['height'])

        coord1 = (x/width, y/height)
        coord2 = ((x+w)/width, (y+h)/height)
        offset = (float(infos['xoffset']), float(infos['yoffset']))
        gwidth = int(infos['xadvance'])

        lst_file.write(
            '{}\t{}\t{}\t{}\t{}\t{}\t{}\t{}\n'.format(
                unicode,
                coord1[0], coord1[1],
                coord2[0], coord2[1],
                offset[0], offset[1],
                gwidth))
                
        print('processing glyph: ', unicode, end='\r')

    lst_file.write('{}\n'.format(line_height))
    lst_file.write('{}\t{}\n'.format(spacing[0], spacing[1]))
    lst_file.write('{}\n'.format(scale))
    lst_file.write('{}\n'.format(fallback))

    if(len(kerning_path) > 0 and os.path.exists(kerning_path)):
        kerning = open(kerning_path, 'r', encoding='utf-8').read()
        lst_file.write(kerning)
    
    #kernings_count = int(Line(lines[char_count + 4])['count'])
    #lst_file.write('{}\n'.format(kernings_count))
    #for i in range(char_count + 5, char_count + 5 + kernings_count):
    #    line = lines[i]
    #    infos = Line(line)
    #    first = chr(int(infos['first']))
    #    second = chr(int(infos['second']))
    #    amount = -float(infos['amount'])
    #    lst_file.write('{}\t{}\t{}\n'.format(first, second, amount))
    #    print('processing kerning: ', first, second, end='\r')
        
    print('processed', char_count, 'glyphes')
    
    lst_file.close()

if __name__ == '__main__':
    import sys
    argsLen = len(sys.argv)
    print(argsLen)
    filename = ''
    if argsLen > 1:
        filename = sys.argv[1]
    else:
        print('Need Arguments')
        sys.exit()
    kerning_path = '' if argsLen < 3 else sys.argv[2]
    scale = 1 if argsLen < 4 else sys.argv[3]
    fallback = '_' if argsLen < 5 else sys.argv[4]
    fnt2lst(filename + '.fnt', filename + '.lst', kerning_path, scale, fallback)
