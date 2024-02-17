import shutil
import zipfile
from weasyprint import HTML, CSS, default_url_fetcher
from weasyprint.text.fonts import FontConfiguration

class my_class(object):
    def __init__(self):
        pass
    
    def image_base_url(self):
        
        pass

    def extract_zip_to_temp(self, path):
        with zipfile.ZipFile(path, 'r') as zip_ref:
            ret=zip_ref.extractall("/tmp/epub_temp/")

    def convert(self, file):
        filename = file.split("/")[-1]
        last4 = filename[-4:]
        if(last4!="epub"):
            print("It's a {} file".format(last4))
        quit()
        shutil.copy(file,"/tmp/epub_temp.zip")
        try:
            shutil.rmtree("/tmp/epub_temp")
        except:
            print("")
        os.mkdir('/tmp/epub_temp')
        extract_zip_to_temp("/tmp/epub_temp.zip")
        global_root_dir="/tmp/epub_temp/"
        
        filename=filename[:-4]
        generatepdf()
        shutil.rmtree("/tmp/epub_temp")
    




