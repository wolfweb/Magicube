import math
import sys
from math import floor
from pathlib import Path

from PIL import Image

if __package__:
    import importlib

    raw = importlib.import_module(f"{__package__}.srmd_ncnn_vulkan_wrapper")
else:
    import srmd_ncnn_vulkan_wrapper as raw


class Srmd:
    def __init__(
        self,
        gpuid=0,
        model="models-srmd",
        tta_mode=False,
        scale: float = 2,
        noise: int = 3,
        tilesize=0,
    ):
        """
        Srmd class which can do image super resolution.

        :param gpuid: the id of the gpu device to use.
        :param model: the name or the path to the model
        :param tta_mode: whether to enable tta mode or not
        :param scale: scale ratio. value: float. default: 2
        :param noise: denoise level. value: int ranges[-1, 8]. default: 3
        :param tilesize: tile size. 0 for automatically setting the size. default: 0
        """
        self._raw_srmd = raw.SRMDWrapper(gpuid, tta_mode)
        self.model = model
        self.gpuid = gpuid
        self.scale = scale  # the real scale ratio
        self.noise = noise
        self.tilesize = tilesize if tilesize > 0 else self.get_optimal_tilesize()
        self.set_prepadding()

    @property
    def scale(self):
        return self._scale

    @scale.setter
    def scale(self, scale):
        self._scale = scale
        pre_raw_scale = self._raw_srmd.scale
        self._raw_srmd.scale = max(
            2, min(math.ceil(scale), 4)
        )  # limit the scale ratio of raw SRMD object from 2 to 4
        self.set_prepadding()
        if pre_raw_scale != self._raw_srmd.scale:
            self.load()

    @property
    def noise(self):
        return self._noise

    @noise.setter
    def noise(self, noise):
        self._noise = noise
        pre_raw_noise = self._raw_srmd.noise
        self._raw_srmd.noise = noise
        self.set_prepadding()
        if pre_raw_noise != self._raw_srmd.noise:
            self.load()

    @property
    def tilesize(self):
        return self._tilesize

    @tilesize.setter
    def tilesize(self, tilesize: int):
        if tilesize > 0:
            self._tilesize = tilesize
        else:
            self._tilesize = self.get_optimal_tilesize()

        self._raw_srmd.tilesize = self._tilesize

    def load(self, parampath: str = "", modelpath: str = "") -> None:
        """
        Load models from given paths. Use self.model if one or all of the parameters are not given.

        :param parampath: the path to model params. usually ended with ".param"
        :param modelpath: the path to model bin. usually ended with ".bin"
        :return: None
        """
        if not parampath or not modelpath:
            model_dir = Path(self.model)
            if not model_dir.is_absolute():
                if (
                    not model_dir.is_dir()
                ):  # try to load it from module path if not exists as directory
                    dir_path = Path(__file__).parent
                    model_dir = dir_path.joinpath("models", self.model)

            if self._raw_srmd.noise == -1:
                parampath = model_dir.joinpath(f"srmdnf_x{self._raw_srmd.scale}.param")
                modelpath = model_dir.joinpath(f"srmdnf_x{self._raw_srmd.scale}.bin")
            else:
                parampath = model_dir.joinpath(f"srmd_x{self._raw_srmd.scale}.param")
                modelpath = model_dir.joinpath(f"srmd_x{self._raw_srmd.scale}.bin")

        if Path(parampath).exists() and Path(modelpath).exists():
            parampath_str, modelpath_str = raw.StringType(), raw.StringType()
            if sys.platform in ("win32", "cygwin"):
                parampath_str.wstr = raw.new_wstr_p()
                raw.wstr_p_assign(parampath_str.wstr, str(parampath))
                modelpath_str.wstr = raw.new_wstr_p()
                raw.wstr_p_assign(modelpath_str.wstr, str(modelpath))
            else:
                parampath_str.str = raw.new_str_p()
                raw.str_p_assign(parampath_str.str, str(parampath))
                modelpath_str.str = raw.new_str_p()
                raw.str_p_assign(modelpath_str.str, str(modelpath))

            self._raw_srmd.load(parampath_str, modelpath_str)
        else:
            raise FileNotFoundError(f"{parampath} or {modelpath} not found")

    def process(self, im: Image) -> Image:
        """
        Upscale the given PIL.Image, and will call RealSR.process() more than once for scale ratios greater than 4

        :param im: PIL.Image
        :return: PIL.Image
        """
        if self.scale > 1:
            cur_scale = 1
            w, h = im.size

            while cur_scale < self.scale / 4:
                im = self._process(im)
                cur_scale *= self._raw_srmd.scale

            w, h = floor(w * self.scale), floor(h * self.scale)
            im = im.resize((w, h))

        return im

    def _process(self, im: Image) -> Image:
        """
        Call RealSR.process() once for the given PIL.Image

        :param im: PIL.Image
        :return: PIL.Image
        """
        in_bytes = bytearray(im.tobytes())
        channels = int(len(in_bytes) / (im.width * im.height))
        out_bytes = bytearray((self._raw_srmd.scale ** 2) * len(in_bytes))

        raw_in_image = raw.Image(in_bytes, im.width, im.height, channels)
        raw_out_image = raw.Image(
            out_bytes,
            self._raw_srmd.scale * im.width,
            self._raw_srmd.scale * im.height,
            channels,
        )

        self._raw_srmd.process(raw_in_image, raw_out_image)

        return Image.frombytes(
            im.mode,
            (self._raw_srmd.scale * im.width, self._raw_srmd.scale * im.height),
            bytes(out_bytes),
        )

    def get_optimal_tilesize(self):
        heap_budget = raw.get_heap_budget(self.gpuid)
        if "models-srmd" in self.model:
            if heap_budget > 2600:
                return 400
            elif heap_budget > 740:
                return 200
            elif heap_budget > 250:
                return 100
            else:
                return 32
        else:
            raise NotImplementedError(f'model "{self.model}" is not supported')

    def set_prepadding(self):
        self._raw_srmd.prepadding = 12


class SRMD(Srmd):
    ...
