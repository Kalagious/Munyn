import os
import re

files = os.listdir()
outFileName = "../IconDictionary.axaml"
outFile = open(outFileName,'w')
outFile.close()


outFile = open(outFileName,'w')
outFile.write('<ResourceDictionary xmlns="https://github.com/avaloniaui"\nxmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">\n')

for file in files:
	fileData = open(file, 'r').read()
	fileData = fileData.replace('\x00', '')
	matches = re.findall(' d="(.*?)"\/>', fileData)
	if matches != None and len(matches) > 0:
		pathData = ""
	
		for i in range(0,len(matches)):
			pathData += matches[i]

		resourceString = f'<StreamGeometry x:Key="{file.split('.')[0]}">{pathData}</StreamGeometry>'
		outFile.write(resourceString+'\n')
		print(file)

outFile.write('\n</ResourceDictionary>')

