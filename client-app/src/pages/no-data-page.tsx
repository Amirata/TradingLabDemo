export default function NoDataPage({message}: { message: React.ReactNode }) {
    return (
        <div className="flex flex-col items-center justify-center min-h-[685px]">
            <div className="flex font-iran-sans text-2xl text-white/80">
                {message}
            </div>
        </div>
    );
}
