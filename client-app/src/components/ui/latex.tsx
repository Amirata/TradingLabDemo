import React, { useEffect, useRef } from "react";
import katex from "katex";
import "katex/dist/katex.min.css"; // استایل‌های KaTeX

const Latex: React.FC<{ latex: string }> = ({ latex }) => {
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (containerRef.current) {
            katex.render(latex, containerRef.current, {
                throwOnError: false,
            });
        }
    }, [latex]);

    return <div dir="ltr" className="text-white/80" ref={containerRef} />;
};

export default Latex;