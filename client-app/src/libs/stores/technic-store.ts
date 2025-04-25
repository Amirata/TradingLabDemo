import { create } from "zustand";
import { PaginationResult, Technic } from "../definitions";

type State = {
  technics: Technic[];
  pageIndex: number;
  pageSize: number;
  count: number;
};

type Actions = {
  setData: (res: PaginationResult<Technic>) => void;
  //setCurrentPrice: (auctionId: string, amount: number) => void;
};

const initialState: State = {
  technics: [],
  pageIndex: 1,
  pageSize: 10,
  count: 0,
};

export const useTechnicStore = create<State & Actions>((set) => ({
  ...initialState,

  setData: (res: PaginationResult<Technic>) => {
    set(() => ({
      technics: res.data,
      pageIndex: res.pageIndex,
      pageSize: res.pageSize,
      count: res.count,
    }));
  },

  // setCurrentPrice: (auctionId: string, amount: number) => {
  //   set((state) => ({
  //     auctions: state.auctions.map((auction) =>
  //       auction.id === auctionId
  //         ? { ...auction, currentHighBid: amount }
  //         : auction,
  //     ),
  //   }));
  //},
}));
