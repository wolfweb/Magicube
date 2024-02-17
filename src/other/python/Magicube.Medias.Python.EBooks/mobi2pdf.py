import mobi
import sys
import shutil
from weasyprint import HTML, CSS, default_url_fetcher


class my_class(object):
    def __init__(self):
        pass
    
    def convert(self, file):
        tmpdir,filepath = mobi.extract(file)
        image_base = filepath[:-9]
        html = HTML(filename=filepath,base_url=image_base,encoding="utf8")
        filename = file.split("/")[-1]
        html.write_pdf(filename+'.pdf')
        shutil.rmtree(tmpdir)




