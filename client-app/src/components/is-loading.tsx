function IsLoading() {
  return (
    <div className="flex items-center justify-center space-x-2 w-full h-full backdrop-blur-sm ">
      <span className="text-md pl-2 animate-pulse">
        در حال بارگذاری ...
      </span>
      <svg
        className="h-6 w-6 animate-spin stroke-orange-400"
        viewBox="0 0 256 256"
      >
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="128"
          x2="128"
          y1="32"
          y2="64"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="195.9"
          x2="173.3"
          y1="60.1"
          y2="82.7"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="224"
          x2="192"
          y1="128"
          y2="128"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="195.9"
          x2="173.3"
          y1="195.9"
          y2="173.3"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="128"
          x2="128"
          y1="224"
          y2="192"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="60.1"
          x2="82.7"
          y1="195.9"
          y2="173.3"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="32"
          x2="64"
          y1="128"
          y2="128"
        />
        <line
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="24"
          x1="60.1"
          x2="82.7"
          y1="60.1"
          y2="82.7"
        />
      </svg>
    </div>
  );
}

export default IsLoading;
