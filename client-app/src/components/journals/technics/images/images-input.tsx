import React from "react";
import {useDropzone} from "react-dropzone";
import {FieldError, Merge} from "react-hook-form";
import {ImageUp, Trash2} from "lucide-react";

interface ImagesInputProps {
    maxSize?: number;
}

interface ImagesInputProps {
    onChange: (files: File[]) => void;
    value: File[];
    errors: Merge<FieldError, (FieldError | undefined)[]> | undefined;
    previews: string[];
    setPreviews: React.Dispatch<React.SetStateAction<string[]>>;
}

const ImagesInput: React.FC<ImagesInputProps> = ({
                                                     onChange,
                                                     value,
                                                     errors,
                                                     previews,
                                                     setPreviews,
                                                 }) => {
    const {getRootProps, getInputProps} = useDropzone({
        accept: {"image/*": []},
        multiple: true, // Allow multiple file selection
        onDrop: (acceptedFiles) => {
            const newPreviews = acceptedFiles.map((file) =>
                URL.createObjectURL(file),
            );

            setPreviews([...previews, ...newPreviews]);
            onChange([...value, ...acceptedFiles]);
        },
    });

    const renderError = React.useMemo(() => {
        if (errors && Array.isArray(errors)) {
            return (
                <div className="w-full pt-4">
                    <div
                        className=" text-sm border border-red-700 text-red-700 bg-red-700/30 p-5 rounded-sm font-iran-sans-fa-num">
                        <p>تصاویر نا معتبر:</p>
                        <ul>
                            {errors.map((error, index) => (
                                <li key={index}>{`${index + 1}) ${error.message}`}</li>
                            ))}
                        </ul>
                    </div>
                </div>
            );
        }

        return null;
    }, [errors]);
    const handleDelete = (index: number) => {
        const updatedFiles = value.filter((_, i) => i !== index);
        const updatedPreviews = previews.filter((_, i) => i !== index);

        onChange(updatedFiles);
        setPreviews(updatedPreviews);

        // Revoke the preview URL for the removed file
        URL.revokeObjectURL(previews[index]);
    };

    return (
        <div>
            <div {...getRootProps()} className="w-full">
                <input {...getInputProps()} />
                <div
                    className="flex justify-center items-center rounded-sm h-32 bg-zinc-800 p-5 text-white/80 border border-zinc-500">
                    <div>
                        <ImageUp/>
                    </div>
                    <p className="text-lg font-iran-sans pr-2">
                        تصاویر را انتخاب کنید.
                    </p>
                </div>
            </div>
            <div className="flex flex-col items-center justify-center pt-4">

                <div className="w-full rounded-sm min-h-32 bg-zinc-800 p-5 text-white/80 border border-zinc-500 ">
                    <h2 className="font-iran-sans">تصاویر جدید</h2>
                    <div className="flex justify-center items-center flex-wrap pt-5 gap-4">
                        {previews.map((preview, index) => (
                            <div key={index} className="relative flex-none w-32 h-32  overflow-hidden1">
                                <img
                                    alt={`Preview ${index}`}
                                    className="absolute w-full h-full object-cover cursor-pointer"

                                    src={preview}

                                />
                                <button
                                    className="absolute top-1 right-1 z-10 bg-zinc-950/60 p-2 rounded-sm"
                                    type="button"
                                    onClick={() => handleDelete(index)}
                                >
                                    <Trash2 className="size-4"/>
                                </button>
                                <span
                                    className="z-10 flex justify-center items-center bg-zinc-700 opacity-60 rounded-full w-12 h-12 absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2 text-3xl">
                {index + 1}
              </span>
                            </div>
                        ))}
                    </div>
                </div>

                {renderError}

            </div>
        </div>
    );
};

export default ImagesInput;
