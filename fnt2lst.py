#!/usr/bin/env python3
# Windows 平台下应该会根据后缀名判断文件类型

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

def fnt2lst(fnt_path, lst_path, scale=1, fallback=95):
    lst_file = open(lst_path, 'w')
    lines = open(fnt_path, 'r', encoding='utf-8').readlines()

    infos = Line(lines[0][lines[0].find("spacing="):])
    spacing = infos['spacing'].split(',')

    commons = Line(lines[1])
    line_height = int(commons['lineHeight'])
    base = int(commons['base'])
    width = int(commons['scaleW'])
    height = int(commons['scaleH'])

    char_count = int(Line(lines[3])['count']) - 1 # 去掉id=-1的
    lst_file.write('{}\n'.format(char_count))

    for i in range(5, char_count + 4):
        line = lines[i]
        infos = Line(line)
        unicode = int(infos['id'])
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
                offset[0], offset[1] + line_height - base,
                gwidth))
                
        print('processing glyph: ', unicode, end='\r')
    print('processed', char_count, 'glyphes   ')

    lst_file.write('{}\n'.format(line_height))
    lst_file.write('{}\t{}\n'.format(spacing[0], spacing[1]))
    lst_file.write('{}\n'.format(scale))
    lst_file.write('{}\n'.format(fallback))
    
    lst_file.close()

if __name__ == '__main__':
    import sys
    filename = sys.argv[1]
    fnt2lst(filename + '.fnt', filename + '.lst')
