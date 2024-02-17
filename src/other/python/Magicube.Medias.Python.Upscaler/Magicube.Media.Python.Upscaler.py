import sys
import os
import platform

import argparse

# import waifu2x_ncnn_vulkan
# import srmd_ncnn_vulkan
# import realsr_ncnn_vulkan
# import realcugan_ncnn_vulkan

from PIL import Image

from importlib import import_module

ALGORITHM_CLASSES = {
    "anime4k": "anime4k_python.Anime4K",
    "realcugan": "realcugan_ncnn_vulkan.Realcugan",
    "realsr": "realsr_ncnn_vulkan.Realsr",
    "srmd": "srmd_ncnn_vulkan.Srmd",
    "waifu2x": "waifu2x_ncnn_vulkan.Waifu2x",
}

if __name__ == "__main__":
    parser = argparse.ArgumentParser("")
    parser.add_argument("file", help="input image file")
    parser.add_argument("outfile", help="output image file")
    parser.add_argument("--engine", help="upscaling engine", choices=ALGORITHM_CLASSES.keys())
    args = parser.parse_args()

    if not os.path.exists(args.file):
        raise FileNotFoundError(f"{args.file} not found")

    image = Image.open(args.file)
    
    if not args.engine:
        args.engine = "waifu2x"
        
    print("使用{}处理图片=>{}".format(args.engine, args.file))
        
    module_name, class_name = ALGORITHM_CLASSES[args.engine].rsplit(".", 1)
    processor_module = import_module(module_name)
    processor_class = getattr(processor_module, class_name)
    processor_object = processor_class()
    
    im = processor_object.process(image)
    im.save(args.outfile)