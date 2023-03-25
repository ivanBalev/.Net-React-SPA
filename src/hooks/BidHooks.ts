import { Bid } from "./../types/bid";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import Config from "../config";
import axios, { AxiosError, AxiosResponse } from "axios";
import Problem from "../types/problem";

const useFetchBids = (houseId: number) => {
    // Exprected return type is Bid[] // cache key is a combination of 
    // both values in the array
    return useQuery<Bid[], AxiosError<Problem>>(["bids", houseId], () =>
        axios
            .get(`${Config.baseApiUrl}/house/${houseId}/bids`)
            .then((resp) => resp.data)
    );
};

const useAddBid = () => {
    // queryClient gives the ability to invalidate the cache
    const queryClient = useQueryClient();
    return useMutation<AxiosResponse, AxiosError<Problem>, Bid>(
        (b) => axios.post(`${Config.baseApiUrl}/house/${b.houseId}/bids`, b),
        {
            onSuccess: (_, bid) => {
                // No need to navigate since the bid component exists
                // within the House component
                queryClient.invalidateQueries(["bids", bid.houseId]);
            },
        }
    );
};

export { useFetchBids, useAddBid };