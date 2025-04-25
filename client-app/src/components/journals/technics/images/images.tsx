import React from "react";
import { UseFormSetValue } from "react-hook-form";
import {Trash2} from "lucide-react";

type imagesProps = {
  onChange: (images: string[]) => void;
  value: string[];
  setValue: UseFormSetValue<{
    images?: (string | undefined)[] | undefined;
    removedImages?: (string | undefined)[] | undefined;
    newImages?: (File | undefined)[] | undefined;
    id: string;
    name: string;
    description: string;
  }>;
  removedImagesItems: string[];
}

const Images: React.FC<imagesProps> = ({
  onChange,
  value,
  setValue,
  removedImagesItems,
}) => {
  const baseUrl = import.meta.env.VITE_APP_IMG_URL as string;
  return (
    <div className="rounded-sm min-h-32 bg-zinc-800 p-5 text-white/80 border border-zinc-500">
      <h2 className="font-iran-sans">تصاویر موجود</h2>
      <div className="flex justify-center gap-2 pt-5 flex-wrap">
        {value?.map((image, index) => (
          <button
            key={index}
            className="group relative flex-none w-32 h-32 shadow-xl cursor-pointer"
            type="button"
            onClick={() => {
              onChange(value?.filter((img) => img !== image));
              setValue("removedImages", [
                ...removedImagesItems,
                image as string,
              ]);
            }}
          >
            <img
              className="w-full h-full object-cover"
              src={`${baseUrl}${image}`}
            />
            <span className="z-10 opacity-0 flex group-hover:opacity-60 justify-center items-center bg-zinc-700 rounded-full w-12 h-12 absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2 text-3xl transition ease-in-out duration-300">
              <Trash2 />
                {/*<FaTrash className="w-3.5 h-3.5 text-zinc-100" />*/}
            </span>
          </button>
        ))}
      </div>
    </div>
  );
};

export default Images;
