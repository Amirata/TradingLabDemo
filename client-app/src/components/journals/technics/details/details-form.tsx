import {use, useMemo, useState} from "react";
import {BackBtn, PaginationBtn} from "../../../ui/my-buttons.tsx";
import {Technic} from "../../../../libs/definitions.ts";
import {textWithLineBreaks} from "../../../../libs/utilities.ts";
import {ChevronLeft, ChevronRight} from "lucide-react";

type props = {
    dataPromise: Promise<Technic>;
};

type propsImage = {
    images: string[];
    baseUrl: string;
}
const ImageGallery = ({images, baseUrl}: propsImage) => {


    const [selectedIndex, setSelectedIndex] = useState(0);

    const handleSelect = (index: number) => {
        setSelectedIndex(index);
    };

    const handleNext = () => {
        setSelectedIndex((prev) => (prev + 1) % images.length);
    };

    const handlePrev = () => {
        setSelectedIndex((prev) => (prev - 1 + images.length) % images.length);
    };

    return (
        <div className="flex flex-col items-center space-y-4">
            <div className="w-full h-[500px] border flex items-center justify-center bg-black rounded-sm">
                {images.length != 0 && <img src={`${baseUrl}/${images[selectedIndex]}`} alt="Selected image"
                     className="max-w-full max-h-full"/>}
            </div>
            <div
                className="flex space-x-2 mt-5 scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin  scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-x-auto">
                {images.map((img, index) => (
                    <img
                        key={index}
                        src={`${baseUrl}/${img}`}
                        alt={`Thumbnail ${index + 1}`}
                        className={`w-16 h-16 object-cover cursor-pointer border-2 ${
                            selectedIndex === index ? "border-zinc-100" : "border-transparent"
                        }`}
                        onClick={() => handleSelect(index)}
                    />
                ))}
            </div>
            <div className="flex space-x-4">
                <PaginationBtn disabled={selectedIndex == 0} message={<ChevronRight />} onClick={handlePrev} />
                <PaginationBtn disabled={images.length == 0 || selectedIndex == images.length - 1} message={<ChevronLeft />} onClick={handleNext} />
            </div>
        </div>
    );
};

export default function DetailsForm({dataPromise}: props) {
    const baseUrl = import.meta.env.VITE_APP_IMG_URL as string;
    const data = use(dataPromise);

    const linesRender = useMemo(() => {
        if (data) {
            const lines = textWithLineBreaks(data.description);

            return lines.map((line, index) => <p key={index}>{line}</p>);
        }

        return undefined;
    }, [data]);

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">جزئیات تکنیک {data.name}</h1>
                <BackBtn path="/technics"/>
            </div>
            <div className="p-5">

                <div className="flex flex-col gap-4 rounded-sm">
                    <div
                        className="p-5 rounded-sm bg-zinc-700 scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto">
                        <div
                            className="max-h-60 h-60 font-iran-sans-fa-num leading-8 text-justify text-white/80 font-thin">
                            {linesRender}
                        </div>
                    </div>
                    <div className="p-5 rounded-sm bg-zinc-700 ">
                        <ImageGallery images={data.images} baseUrl={baseUrl}/>
                    </div>
                </div>
            </div>
        </>
    );
}