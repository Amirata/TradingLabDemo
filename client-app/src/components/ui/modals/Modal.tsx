import React, {ReactNode, useEffect, useState} from "react";
import {DeleteBtn, SquareBtn} from "../my-buttons.tsx";
import {TriangleAlert, X} from "lucide-react";

interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    children: React.ReactNode;
}

const Modal: React.FC<ModalProps> = ({isOpen, onClose, children}) => {
    const [show, setShow] = useState(false);

    useEffect(() => {
        if (isOpen) {
            setTimeout(() => setShow(true), 10); // Add slight delay for smooth animation
        } else {
            setShow(false);
        }
    }, [isOpen]);

    if (!isOpen) return null;

    return (
        <div
            className={`fixed inset-0 px-5 flex items-center justify-center bg-black/20 backdrop-blur-sm z-50 
      transition-opacity duration-300 ${show ? "opacity-100" : "opacity-0"}`}
        >
            <div
                className={`bg-zinc-800 p-4 rounded-sm border border-zinc-600 min-w-96 relative transform transition-all duration-300
        ${show ? "scale-100 opacity-100" : "scale-90 opacity-0"}`}
            >
                {/* Close Button */}
                <SquareBtn message={<X/>} onClick={onClose}/>
                {children}
            </div>
        </div>
    );
};


export const DeleteModal = (
    {
        message,
        isModalOpen,
        setIsModalOpen,
        onDelete,
        isLoading

    }: {
        message: string,
        isModalOpen: boolean,
        setIsModalOpen: React.Dispatch<React.SetStateAction<boolean>>,
        onDelete: () => void,
        isLoading: boolean
    }
) => {
    return (<Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
        <div className="flex flex-col mt-6 ">
            <div className="flex justify-between items-center">
                <div>
                    <p className="text-white/80 text-sm font-iran-sans-fa-num">{message}</p>
                </div>
                <div>
                    <TriangleAlert className="size-8 stroke-white/80"/>
                </div>
            </div>
            <div className="self-center mt-5">
                <DeleteBtn onDelete={onDelete} isLoading={isLoading}/>
            </div>
        </div>

    </Modal>)
}

export const HelpModal = (
    {
        message,
        isModalOpen,
        setIsModalOpen
    }: {
        message: ReactNode,
        isModalOpen: boolean,
        setIsModalOpen: React.Dispatch<React.SetStateAction<boolean>>
    }
) => {
    return (<Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
        <div className="flex flex-col mt-6 ">
            {message}
        </div>
    </Modal>)
}
