import { useEffect, useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { PaginationBtn } from "../my-buttons.tsx";

interface Props {
  lastPage: number;
  numberOfButton: number;
  handlePageClick: (page: number) => void;
  currentPage: number;
}

export default function Pagination({ lastPage, numberOfButton, handlePageClick, currentPage }: Props) {
  const [pages, setPages] = useState<number[]>([]);
  const [selectedPage, setSelectedPage] = useState(currentPage);

  useEffect(() => {
    setSelectedPage(currentPage);
  }, [currentPage]);

  useEffect(() => {
    const numButtons = Math.max(3, Math.min(numberOfButton, lastPage));
    const adjustedButtons = numButtons % 2 === 0 ? numButtons - 1 : numButtons;
    setPages(generatePageNumbers(1, adjustedButtons, lastPage));
  }, [lastPage, numberOfButton]);

  const generatePageNumbers = (start: number, count: number, max: number) =>
      Array.from({ length: Math.min(count, max - start + 1) }, (_, i) => start + i);

  const updatePages = (newPage: number) => {
    if (newPage <= pages[0] || newPage >= pages[pages.length - 1]) {
      const halfRange = Math.floor(pages.length / 2);
      const start = Math.max(1, Math.min(newPage - halfRange, lastPage - pages.length + 1));
      setPages(generatePageNumbers(start, pages.length, lastPage));
    }
  };

  const handleClick = (page: number) => {
    setSelectedPage(page);
    handlePageClick(page);
    updatePages(page);
  };

  return (
      <nav aria-label="Page navigation">
        <ul className="inline-flex">
          <li>
            <PaginationBtn disabled={currentPage == 1} message={<ChevronRight />} onClick={() => handleClick(1)} />
          </li>
          {pages.map((page) => (
              <li key={page}>
                <PaginationBtn
                    onClick={() => handleClick(page)}
                    message={page}
                    isSelected={page === selectedPage}
                />
              </li>
          ))}
          <li>
            <PaginationBtn disabled={currentPage == lastPage} message={<ChevronLeft />} onClick={() => handleClick(lastPage)} />
          </li>
        </ul>
      </nav>
  );
}
