export const Loader = () => {
    return (
        <div className="flex flex-col h-screen justify-center items-center">
            <div className="flex space-x-1">
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.4s]"></div>
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.3s]"></div>
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.2s] "></div>
            </div>
        </div>
    );
};

export const ChartLoader = () => {
    return (
        <div className="flex flex-col h-full justify-center items-center">
            <div className="flex space-x-1">
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.4s]"></div>
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.3s]"></div>
                <div className="w-2 h-6 bg-blue-500 animate-[bounce_.8s_infinite_ease-in-out_.2s] "></div>
            </div>
        </div>
    );
};