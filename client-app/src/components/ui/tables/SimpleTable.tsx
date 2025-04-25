import { Fragment, ReactNode } from "react";
import {ChevronDown, ChevronUp, ChevronsUpDown} from "lucide-react";
import { Sort, PaginatedResult } from "../../../libs/definitions.ts";
import Pagination from "../paginations/Pagination.tsx";

export interface Config<T> {
  key: number;
  label: string;
  header?: () => ReactNode;
  render: (arg: T) => string | number | ReactNode;
  sortValue?: (arg: T) => string | number;
  sortName?: string;
  width?: string;
  direction?: string;
  fontFamily?: string;
  dir?: string;
}

interface Props<T> {
  data: PaginatedResult<T>;
  config: Array<Config<T>>;
  rowKeyFn: (model: T) => string | number;
  columnKeyFn: (config: Config<T>, model: T) => string;
  onPageClick: (page: number) => void;
  onSearch: (page: string) => void;
  search: string;
  onHeaderSortClick: (label: string) => void;
  numberOfButton: number;
  sorts: Sort[];
  isLoading: boolean;
}

function SimpleTable<T>({
  data,
  config,
  rowKeyFn,
  columnKeyFn,
  onPageClick,
  onSearch,
  search,
  onHeaderSortClick,
  numberOfButton,
  sorts,
  isLoading,
}: Props<T>) {
  const tableHeader = (
    <div className="flex items-center py-4 text-xs">
      <div className="ml-auto flex rounded-full px-2 bg-slate-200 focus-within:bg-white/80 transition-colors duration-500">
        <input
          value={search}
          onChange={(e) => onSearch(e.currentTarget.value)}
          type="text"
          className="input  w-36 text-slate-500 placeholder:text-slate-400 bg-transparent rounded-full border-0"
          placeholder="جستجو ..."
        />
        <div className="w-6">
          {search && (
            <button
              onClick={() => onSearch("")}
              type="button"
              className="cursor-pointer h-full flex items-center text-slate-400 outline-none focus:outline-none"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="100%"
                height="100%"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
                className="feather feather-x w-4 h-4"
              >
                <line x1="18" y1="6" x2="6" y2="18"></line>
                <line x1="6" y1="6" x2="18" y2="18"></line>
              </svg>
            </button>
          )}
        </div>
      </div>
      <div>
        {isLoading && (
          <div className="flex items-center space-x-2">
            <span className="text-xs text-indigo-500 pl-2 animate-pulse">
              درحال بارگذاری ...
            </span>
            <svg
              className="h-5 w-5 animate-spin stroke-purple-500"
              viewBox="0 0 256 256"
            >
              <line
                x1="128"
                y1="32"
                x2="128"
                y2="64"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="195.9"
                y1="60.1"
                x2="173.3"
                y2="82.7"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="224"
                y1="128"
                x2="192"
                y2="128"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="195.9"
                y1="195.9"
                x2="173.3"
                y2="173.3"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="128"
                y1="224"
                x2="128"
                y2="192"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="60.1"
                y1="195.9"
                x2="82.7"
                y2="173.3"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="32"
                y1="128"
                x2="64"
                y2="128"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
              <line
                x1="60.1"
                y1="60.1"
                x2="82.7"
                y2="82.7"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="24"
              ></line>
            </svg>
          </div>
        )}
      </div>
    </div>
  );

  const updatedConfig = config.map((column) => {
    if (!column.sortValue) {
      return {
        ...column,
        header: () => (
          <th
            className={`p-1 bg-slate-100 select-none h-2 text-xs ${column.width}`}
          >
            <div className="flex items-center truncate">{column.label}</div>
          </th>
        ),
      };
    }

    return {
      ...column,
      header: () => (
        <th
          className={`cursor-pointer p-1 bg-slate-100  hover:bg-slate-200 active:bg-slate-300 select-none h-2 text-xs ${column.width}`}
          onClick={() => onHeaderSortClick(column.sortName!)}
        >
          <div className="flex items-center truncate ">
            {getIcons(column.sortName!, sorts)}
            {column.label}
          </div>
        </th>
      ),
    };
  });

  const renderedHeaders = updatedConfig.map((column) => {
    if (column.header) {
      return <Fragment key={column.key}>{column.header()}</Fragment>;
    }

    return <th key={column.key}>{column.label}</th>;
  });

  const sortedData = data.data;

  let renderedRows: ReactNode[];

  if (sortedData.length === 0) {
    renderedRows = [
      <tr className="rounded-lg odd:bg-white/80 " key={1}>
        <td
          className="p-1 h-72 text-xs truncate first:rounded-r-lg last:rounded-l-lg my-2 text-center "
          colSpan={100}
        >
          یافت نشد!
        </td>
      </tr>,
    ];
  } else {
    renderedRows = sortedData.map((rowData) => {
      const renderedCells = updatedConfig.map((column) => {
        const key = columnKeyFn(column, rowData);
        return (
          <td
            className={`p-1 border border-slate-300 hover:bg-amber-200 max-h-8 text-xs truncate my-1 ${
              key.charAt(0) == "g"
                ? "animate-cellGreen"
                : key.charAt(0) == "r"
                ? "animate-cellRed"
                : ""
            } ${column.direction ? column.direction : ""} ${
              column.fontFamily ? column.fontFamily : ""
            }`}
            key={key}
            dir={column.dir ? column.dir : "rtl"}
          >
            {column.render(rowData)}
          </td>
        );
      });

      return (
        <tr
          className="odd:bg-white/50 hover:bg-amber-100 animate-color"
          key={rowKeyFn(rowData)}
        >
          {renderedCells}
        </tr>
      );
    });
  }

  return (
    <div className="flex h-full flex-col justify-stretch overflow-x-auto rounded-lg p-2 bg-slate-100">
      <div className="basis-1/2">
        {tableHeader}
        <table className="table-auto w-full">
          <thead>
            <tr>{renderedHeaders}</tr>
          </thead>
          <tbody>{renderedRows}</tbody>
        </table>
      </div>
      <div className="basis-1/2 flex flex-col justify-end">

      <div className="self-center py-4 flex justify-between w-full items-center">
        <div className=" text-slate-400 h-8 flex items-center text-xs basis-1/3">
          {data.totalCount !== 0 &&
            `${data.pageSize * data.currentPage - data.pageSize + 1} تا ${
              data.totalCount <= data.pageSize * data.currentPage
                ? data.totalCount
                : data.pageSize * data.currentPage
            } `}
        </div>
        {data.totalCount !== 0 && (
          <Pagination
            lastPage={data.totalPages}
            numberOfButton={numberOfButton}
            handlePageClick={onPageClick}
            currentPage={data.currentPage}
          />
        )}

        <div className=" text-slate-400 h-8 flex items-center justify-end text-xs basis-1/3 text-left">
          {` تعداد کل ${data.totalCount}`}
        </div>
      </div>
      </div>
    </div>
  );
}

function getIcons(sortName: string, orders: Sort[]) {
  const order = orders.filter((order) => order.sortBy === sortName)[0];

  if (order === undefined) {
    return (
      <div>
        <ChevronsUpDown />
      </div>
    );
  } else {
    if (order.sortOrder === "asc") {
      return (
        <>
          <div className="text-teal-400">
              <ChevronUp />
          </div>
          <div className="px-1 text-indigo-400">{`(${order.order})`}</div>
        </>
      );
    } else if (order.sortOrder === "desc") {
      return (
        <>
          <div className="text-rose-400">
              <ChevronDown />
          </div>
          <div className="px-1 text-indigo-400">{`(${order.order})`}</div>
        </>
      );
    }
  }
}

export default SimpleTable;
