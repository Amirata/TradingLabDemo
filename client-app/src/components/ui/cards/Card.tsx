import { ReactNode } from "react";

interface Props {
  children: ReactNode;
  isLoading?: boolean;
}

export default function Card({ children, isLoading }: Props) {
  //console.log(isLoading);
  return (
    <div className="relative bg-white rounded-lg h-min-full">
      {isLoading && (
        <div className="absolute flex items-center justify-center z-20 bg-white/40 backdrop-blur-sm h-full w-full animate-[fade_1s]">
          <span className="select-none ml-2 text-slate-400">
            در حال بارگذاری
          </span>
          <span className="ml-1 animate-[up-down_1s_ease-in-out_.5s_infinite] w-2 h-2 bg-slate-400 rounded-full"></span>
          <span className="ml-1 animate-[up-down_1s_ease-in-out_.7s_infinite] w-2 h-2 bg-slate-400 rounded-full delay-100"></span>
          <span className="ml-1 animate-[up-down_1s_ease-in-out_.9s_infinite] w-2 h-2 bg-slate-400 rounded-full "></span>
        </div>
      )}
      {children}
    </div>
  );
}
