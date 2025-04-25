import {Fragment, ReactNode} from "react";
import clsx from "clsx";
import Pagination from "../paginations/Pagination.tsx";
import {ChevronDown, ChevronUp, ChevronsUpDown, X} from "lucide-react";
import {Sort, PaginatedResult} from "../../../libs/definitions.ts";

export interface Config<T> {
    key: number;
    label: string;
    render: (arg: T) => ReactNode;
    sortValue?: (arg: T) => string | number;
    sortName?: string;
    width?: string;
    direction?: string;
    fontFamily?: string;
}

interface Props<T> {
    data: PaginatedResult<T> | undefined;
    config: Config<T>[];
    keyFn: (arg: T) => string | number;
    onPageClick: (page: number) => void;
    onSearch: (query: string) => void;
    search: string;
    onHeaderSortClick: (label: string) => void;
    numberOfButton: number;
    sorts: Sort[];
    loading: boolean;
    searchPlaceholder: string;
    headerContent?: ReactNode;
}

function Table<T>({
                      data,
                      config,
                      keyFn,
                      onPageClick,
                      onSearch,
                      search,
                      onHeaderSortClick,
                      numberOfButton,
                      sorts,
                      loading,
                      searchPlaceholder,
                      headerContent = undefined
                  }: Props<T>) {
    return (
        <div className="flex flex-col min-h-[500px] rounded-lg p-5 bg-zinc-700 font-iran-sans-fa-num">
            <div className="flex justify-between items-center">
                <TableSearch search={search} searchPlaceholder={searchPlaceholder} onSearch={onSearch}/>
                <div>
                    {headerContent}
                </div>
            </div>
            <div
                className="flex-1 overflow-auto scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin  scrollbar-thumb-zinc-600 scrollbar-track-zinc-500">
                {/*<table className="table-fixed w-full border-collapse">*/}
                <table className="table-fixed min-w-full border-collapse">
                    <thead className="sticky top-0 bg-zinc-800">
                    <tr>
                        {config.map((column) => (
                            <Fragment key={column.key}>
                                <TableHeader column={column} sorts={sorts} onHeaderSortClick={onHeaderSortClick}/>
                            </Fragment>
                        ))}
                    </tr>
                    </thead>
                    <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan={config.length} className="p-2 h-[250px] text-sm text-center text-white/80">
                                در حال بارگذاری...
                            </td>
                        </tr>
                    ) : data?.data.length ? (
                        data.data.map((rowData) => <TableRow key={keyFn(rowData)} rowData={rowData} config={config}/>)
                    ) : (
                        <tr>
                            <td colSpan={config.length} className="p-2 h-[250px] text-sm text-center text-white/80"
                                dir="rtl">
                                داده ای یافت نشد!
                            </td>
                        </tr>
                    )}
                    </tbody>
                </table>
            </div>
            <TableFooter data={data} onPageClick={onPageClick} numberOfButton={numberOfButton}/>
        </div>
    );
}

const TableSearch = ({search, searchPlaceholder, onSearch}: {
    search: string;
    searchPlaceholder: string;
    onSearch: (query: string) => void
}) => (
    <div className="flex items-center py-4">
        <div
            className="ml-auto flex rounded-sm px-2 bg-zinc-700 focus-within:bg-zinc-500 transition-colors duration-500">
            <input
                value={search}
                onChange={(e) => onSearch(e.currentTarget.value)}
                type="text"
                className="outline-none min-h-10 text-white/80 placeholder:text-zinc-400 bg-transparent border-0"
                placeholder={searchPlaceholder}
            />
            <div className="w-4 min-h-10 flex items-center">
                {search && (
                    <button onClick={() => onSearch("")}>
                        <X className="size-4 stroke-white/80"/>
                    </button>
                )}
            </div>
        </div>
    </div>
);

const TableHeader = ({column, sorts, onHeaderSortClick}: {
    column: Config<any>;
    sorts: Sort[];
    onHeaderSortClick: (label: string) => void
}) => (
    <th
        className={clsx(
            "p-4 h-10 text-white/80 select-none text-sm",
            column.width,
            column.sortName && "cursor-pointer hover:bg-zinc-600",
            "border-b border-zinc-600"
        )}
        onClick={column.sortName ? () => onHeaderSortClick(column.sortName!) : undefined}
    >
        <div className="flex items-center truncate">
            {column.sortName && getIcons(column.sortName, sorts)}
            {column.label}
        </div>
    </th>
);

const TableRow = <T, >({rowData, config}: { rowData: T; config: Config<T>[] }) => (
    <tr className="text-white/80 odd:bg-zinc-600/40 even:bg-zinc-800/40 animate-color h-10 border-b border-zinc-600">
        {config.map((column) => (
            <td
                key={column.key}
                className={clsx(
                    "p-2 h-10 text-sm truncate box-border",
                    column.direction,
                    column.fontFamily
                )}
            >
                {column.render(rowData)}
            </td>
        ))}
    </tr>
);

const TableFooter = ({data, onPageClick, numberOfButton}: {
    data: PaginatedResult<any> | undefined;
    onPageClick: (page: number) => void;
    numberOfButton: number
}) => (
    data &&
    <div className="self-center py-4 flex justify-between w-full items-center border-t border-zinc-600 mt-4">
        <div className="text-white/80 text-xs basis-1/3">
            {data.totalCount !== 0 && `${data.pageSize * (data.currentPage - 1) + 1} تا ${Math.min(data.pageSize * data.currentPage, data.totalCount)}`}
        </div>
        {data?.totalCount !== 0 &&
            <Pagination lastPage={data.totalPages} numberOfButton={numberOfButton} handlePageClick={onPageClick}
                        currentPage={data.currentPage}/>}
        <div className="text-white/80 text-xs basis-1/3 text-left">{` تعداد کل ${data.totalCount}`}</div>
    </div>
);

const getIcons = (sortName: string, sorts: Sort[]) => {
    const order = sorts.find((s) => s.sortBy === sortName);
    if (!order) return <ChevronsUpDown className="size-4 stroke-white/80"/>;
    return order.sortOrder === "asc" ?
        <>
            <ChevronUp className="size-4 stroke-white/80"/>
            <div className="p-1 text-white/80">{`(${order.order})`}</div>
        </>
        :
        <>
            <ChevronDown className="size-4 stroke-white/80"/>
            <div className="p-1 text-white/80">{`(${order.order})`}</div>
        </>;
};

export default Table;