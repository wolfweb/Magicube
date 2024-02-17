# This file was automatically generated by SWIG (https://www.swig.org).
# Version 4.1.1
#
# Do not make changes to this file unless you know what you are doing - modify
# the SWIG interface file instead.

from sys import version_info as _swig_python_version_info
# Import the low-level C/C++ module
if __package__ or "." in __name__:
    from . import _realsr_ncnn_vulkan_wrapper
else:
    import _realsr_ncnn_vulkan_wrapper

try:
    import builtins as __builtin__
except ImportError:
    import __builtin__

def _swig_repr(self):
    try:
        strthis = "proxy of " + self.this.__repr__()
    except __builtin__.Exception:
        strthis = ""
    return "<%s.%s; %s >" % (self.__class__.__module__, self.__class__.__name__, strthis,)


def _swig_setattr_nondynamic_instance_variable(set):
    def set_instance_attr(self, name, value):
        if name == "this":
            set(self, name, value)
        elif name == "thisown":
            self.this.own(value)
        elif hasattr(self, name) and isinstance(getattr(type(self), name), property):
            set(self, name, value)
        else:
            raise AttributeError("You cannot add instance attributes to %s" % self)
    return set_instance_attr


def _swig_setattr_nondynamic_class_variable(set):
    def set_class_attr(cls, name, value):
        if hasattr(cls, name) and not isinstance(getattr(cls, name), property):
            set(cls, name, value)
        else:
            raise AttributeError("You cannot add class attributes to %s" % cls)
    return set_class_attr


def _swig_add_metaclass(metaclass):
    """Class decorator for adding a metaclass to a SWIG wrapped class - a slimmed down version of six.add_metaclass"""
    def wrapper(cls):
        return metaclass(cls.__name__, cls.__bases__, cls.__dict__.copy())
    return wrapper


class _SwigNonDynamicMeta(type):
    """Meta class to enforce nondynamic attributes (no new attributes) for a class"""
    __setattr__ = _swig_setattr_nondynamic_class_variable(type.__setattr__)



def new_str_p():
    return _realsr_ncnn_vulkan_wrapper.new_str_p()

def copy_str_p(value):
    return _realsr_ncnn_vulkan_wrapper.copy_str_p(value)

def delete_str_p(obj):
    return _realsr_ncnn_vulkan_wrapper.delete_str_p(obj)

def str_p_assign(obj, value):
    return _realsr_ncnn_vulkan_wrapper.str_p_assign(obj, value)

def str_p_value(obj):
    return _realsr_ncnn_vulkan_wrapper.str_p_value(obj)

def new_wstr_p():
    return _realsr_ncnn_vulkan_wrapper.new_wstr_p()

def copy_wstr_p(value):
    return _realsr_ncnn_vulkan_wrapper.copy_wstr_p(value)

def delete_wstr_p(obj):
    return _realsr_ncnn_vulkan_wrapper.delete_wstr_p(obj)

def wstr_p_assign(obj, value):
    return _realsr_ncnn_vulkan_wrapper.wstr_p_assign(obj, value)

def wstr_p_value(obj):
    return _realsr_ncnn_vulkan_wrapper.wstr_p_value(obj)
class RealSR(object):
    thisown = property(lambda x: x.this.own(), lambda x, v: x.this.own(v), doc="The membership flag")
    __repr__ = _swig_repr

    def __init__(self, gpuid, tta_mode=False, num_threads=1):
        _realsr_ncnn_vulkan_wrapper.RealSR_swiginit(self, _realsr_ncnn_vulkan_wrapper.new_RealSR(gpuid, tta_mode, num_threads))
    __swig_destroy__ = _realsr_ncnn_vulkan_wrapper.delete_RealSR
    scale = property(_realsr_ncnn_vulkan_wrapper.RealSR_scale_get, _realsr_ncnn_vulkan_wrapper.RealSR_scale_set)
    tilesize = property(_realsr_ncnn_vulkan_wrapper.RealSR_tilesize_get, _realsr_ncnn_vulkan_wrapper.RealSR_tilesize_set)
    prepadding = property(_realsr_ncnn_vulkan_wrapper.RealSR_prepadding_get, _realsr_ncnn_vulkan_wrapper.RealSR_prepadding_set)

# Register RealSR in _realsr_ncnn_vulkan_wrapper:
_realsr_ncnn_vulkan_wrapper.RealSR_swigregister(RealSR)
class Image(object):
    thisown = property(lambda x: x.this.own(), lambda x, v: x.this.own(v), doc="The membership flag")
    __repr__ = _swig_repr
    data = property(_realsr_ncnn_vulkan_wrapper.Image_data_get, _realsr_ncnn_vulkan_wrapper.Image_data_set)
    w = property(_realsr_ncnn_vulkan_wrapper.Image_w_get, _realsr_ncnn_vulkan_wrapper.Image_w_set)
    h = property(_realsr_ncnn_vulkan_wrapper.Image_h_get, _realsr_ncnn_vulkan_wrapper.Image_h_set)
    elempack = property(_realsr_ncnn_vulkan_wrapper.Image_elempack_get, _realsr_ncnn_vulkan_wrapper.Image_elempack_set)

    def __init__(self, d, w, h, channels):
        _realsr_ncnn_vulkan_wrapper.Image_swiginit(self, _realsr_ncnn_vulkan_wrapper.new_Image(d, w, h, channels))
    __swig_destroy__ = _realsr_ncnn_vulkan_wrapper.delete_Image

# Register Image in _realsr_ncnn_vulkan_wrapper:
_realsr_ncnn_vulkan_wrapper.Image_swigregister(Image)
class StringType(object):
    thisown = property(lambda x: x.this.own(), lambda x, v: x.this.own(v), doc="The membership flag")
    __repr__ = _swig_repr
    str = property(_realsr_ncnn_vulkan_wrapper.StringType_str_get, _realsr_ncnn_vulkan_wrapper.StringType_str_set)
    wstr = property(_realsr_ncnn_vulkan_wrapper.StringType_wstr_get, _realsr_ncnn_vulkan_wrapper.StringType_wstr_set)

    def __init__(self):
        _realsr_ncnn_vulkan_wrapper.StringType_swiginit(self, _realsr_ncnn_vulkan_wrapper.new_StringType())
    __swig_destroy__ = _realsr_ncnn_vulkan_wrapper.delete_StringType

# Register StringType in _realsr_ncnn_vulkan_wrapper:
_realsr_ncnn_vulkan_wrapper.StringType_swigregister(StringType)
class RealSRWrapped(RealSR):
    thisown = property(lambda x: x.this.own(), lambda x, v: x.this.own(v), doc="The membership flag")
    __repr__ = _swig_repr

    def __init__(self, gpuid, tta_mode=False, num_threads=1):
        _realsr_ncnn_vulkan_wrapper.RealSRWrapped_swiginit(self, _realsr_ncnn_vulkan_wrapper.new_RealSRWrapped(gpuid, tta_mode, num_threads))

    def load(self, parampath, modelpath):
        return _realsr_ncnn_vulkan_wrapper.RealSRWrapped_load(self, parampath, modelpath)

    def process(self, inimage, outimage):
        return _realsr_ncnn_vulkan_wrapper.RealSRWrapped_process(self, inimage, outimage)
    __swig_destroy__ = _realsr_ncnn_vulkan_wrapper.delete_RealSRWrapped

# Register RealSRWrapped in _realsr_ncnn_vulkan_wrapper:
_realsr_ncnn_vulkan_wrapper.RealSRWrapped_swigregister(RealSRWrapped)

def get_gpu_count():
    return _realsr_ncnn_vulkan_wrapper.get_gpu_count()

def get_heap_budget(gpuid):
    return _realsr_ncnn_vulkan_wrapper.get_heap_budget(gpuid)

