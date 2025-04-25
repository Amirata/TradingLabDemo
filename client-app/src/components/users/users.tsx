import {useEffect, useState, useCallback} from "react";
import {Link} from "react-router-dom";
import {SquarePlus} from "lucide-react";

import Table, {Config} from "../ui/tables/Table";
import {SquareBtn, TableAddBtn} from "../ui/my-buttons.tsx";
import apisWrapper from "../../libs/apis-wrapper.ts";


import {
    Sort,
    PaginationDto,
    PaginatedResult,
    User, QueryDto,
} from "../../libs/definitions.ts";
import {DeleteIcon, EditIcon, EyeIcon} from "../icons.tsx";
import {DeleteModal} from "../ui/modals/Modal.tsx";


const useUsersData = (trigger: boolean) => {
    const [page, setPage] = useState<PaginationDto>({pageSize: 5, pageNumber: 1});
    const [search, setSearch] = useState<string>("");
    const [data, setData] = useState<PaginatedResult<User> | undefined>();
    const [isLoading, setIsLoading] = useState(true);
    const [sorts, setSorts] = useState<Sort[]>([]);

    useEffect(() => {

        setIsLoading(true);
        const queryDto: QueryDto = {...page, search, sorts};

        apisWrapper.UserWrapper.list(queryDto)
            .then((newData) => {
                setData(newData);
            })
            .catch(console.error)
            .finally(() => setIsLoading(false));
    }, [page, search, sorts, trigger]);

    return { data, isLoading, setPage, setSearch, setSorts, search, sorts, page };
};

const useUserDelete = (setPage: (setter: (prev: PaginationDto) => PaginationDto) => void,
                       currentPage: PaginationDto,
                       dataLength: number,
                       setUpdateTableData: (setter: (prev: boolean) => boolean) => void // اضافه کردن setUpdateTableData
)=> {
    const [deleteUser, setDeleteUser] = useState<User | undefined>(undefined);
    const [deleteLoading, setDeleteLoading] = useState<boolean>(false);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);

    const handleDeleteAlert = <T extends User>(obj: T) => {
        setDeleteUser(obj);
        setIsModalOpen(true);
    };

    const handleDelete = useCallback(() => {
        if (!deleteUser) return;
        setDeleteLoading(true);
        apisWrapper.UserWrapper.delete(deleteUser.id)
            .then(() => {
                if (dataLength === 1 && currentPage.pageNumber > 1) {
                    setPage((prev) => ({
                        ...prev,
                        pageNumber: prev.pageNumber - 1,
                    }));
                }
                setUpdateTableData((prev) => !prev); // استفاده از setUpdateTableData که از Plans اومده
            })
            .catch((error) => {
                console.error("Error deleting Plan:", error);
            })
            .finally(() => {
                setDeleteLoading(false);
                setIsModalOpen(false);
            });
    }, [deleteUser, setPage, currentPage, dataLength, setUpdateTableData]);

    return { deleteLoading, handleDeleteAlert, handleDelete, isModalOpen, setIsModalOpen, deleteUser };
}

const Users = () => {
    const [updateTableData, setUpdateTableData] = useState<boolean>(false);

    const { data, isLoading, setPage, setSearch, setSorts, search, sorts, page } = useUsersData(updateTableData);

    const {
        deleteLoading,
        handleDeleteAlert,
        handleDelete,
        isModalOpen,
        setIsModalOpen,
        deleteUser,
    } = useUserDelete(setPage, page, data?.data.length || 0, setUpdateTableData); // پاس دادن setUpdateTableData

    const handlePageClick = useCallback((pageNumber: number) => {
        setPage((prev) => ({...prev, pageNumber}));
    }, [setPage]);

    const handleSearch = useCallback((search: string) => {
        setSearch(search);
        setPage((prev) => ({...prev, pageNumber: 1}));
    }, [setSearch, setPage]);

    const handleHeaderSortClick = useCallback((sortName: string) => {
        setSorts((prevSorts) => {
            const existingSort = prevSorts.find((sort) => sort.sortBy === sortName);
            const otherSorts = prevSorts.filter((sort) => sort.sortBy !== sortName)
                .map((sort, index) => ({...sort, order: index + 1}));

            if (!existingSort) {
                return [...otherSorts, {sortBy: sortName, sortOrder: "asc", order: otherSorts.length + 1}];
            }
            return existingSort.sortOrder === "asc"
                ? [...otherSorts, {sortBy: sortName, sortOrder: "desc", order: otherSorts.length + 1}]
                : otherSorts;
        });
    }, [setSorts]);

    const tableOperation = (id: string | number, paths: string[], onDeleteFunc: <T, >(obj: T) => void) => (
        <div className="flex items-center justify-end">
            {paths.map((path, index) => {
                if (path === "delete") {
                    return (
                        <SquareBtn key={index} message={<DeleteIcon/>} onClick={onDeleteFunc}/>
                    )
                } else {
                    return (

                        <Link key={index} to={`${path}/${id}`}>
                            <SquareBtn message={[<EyeIcon/>, <EditIcon/>][index]}/>
                        </Link>
                    )
                }
            })}
        </div>
    );


    const config: Array<Config<User>> = [
        {key: 1, width: "w-[25%]", label: "نام کاربری", render: (c) => <span>{c.userName}</span>, sortName: "UserName"},
        {key: 2, width: "w-[50%]", label: "پست الکترونیکی", render: (c) => <div className="w-96 truncate">{(c.email)}</div>},
        {
            key: 3,
            width: "w-[25%]",
            label: "عملیات",
            render: (c) => tableOperation(c.id, ["details", "update", "delete"], () => handleDeleteAlert(c)),
            direction: "text-left"
        },
    ];


    return (
        <div className="p-5 flex flex-col min-h-[616px]">
            <DeleteModal
                setIsModalOpen={setIsModalOpen}
                isModalOpen={isModalOpen}
                onDelete={handleDelete}
                message={`آیا از حذف ${deleteUser?.userName} مطمئن هستید؟`}
                isLoading={deleteLoading}
            />
            <div className="py-6">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">تکنیک ها</h1>
            </div>

            <Table
                data={data}
                config={config}
                keyFn={(c) => c.id}
                onPageClick={handlePageClick}
                onSearch={handleSearch}
                numberOfButton={5}
                onHeaderSortClick={handleHeaderSortClick}
                sorts={sorts}
                search={search}
                loading={isLoading}
                searchPlaceholder={"جستجو در نام کاربری..."}
                headerContent={
                    <TableAddBtn message={
                        <div className="flex justify-center items-center">
                            <SquarePlus strokeWidth={0.75} className="size-4 stroke-white/80 ml-2"/>
                            <span>افزودن</span>
                        </div>
                    }
                                 path="create"
                    />
                }
            />

        </div>
    );
};

export default Users;

