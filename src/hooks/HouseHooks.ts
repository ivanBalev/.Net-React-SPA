import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios, { AxiosError, AxiosResponse } from "axios";
import { useNavigate } from "react-router-dom";
import config from "../config";
import { House } from "../types/house";
import Problem from "../types/problem";

const useFetchHouses = () => {
  // const [houses, setHouses] = useState<House[]>([]);

  // useEffect(() => {
  //   const fetchHouses = async () => {
  //     const rsp = await fetch(`${config.baseApiUrl}/houses`);
  //     const houses = await rsp.json();
  //     setHouses(houses);
  //   }
  //   fetchHouses();
  // }, [])

  // return houses;

  // React-query manages state for us
  return useQuery<House[], AxiosError>(["houses"], () =>
    axios.get(`${config.baseApiUrl}/houses`).then(
      (resp) => resp.data)
  );
}

const useFetchHouse = (id: number) => {
  // Creates cache for each specific id
  // Upon component rerendering, if cache is still valid
  // the data will be taken from it rather than sending requests
  return useQuery<House, AxiosError>(["houses", id], () =>
    axios.get(`${config.baseApiUrl}/house/${id}`).then(
      (resp) => resp.data)
  );
}

const useAddHouse = () => {
  const nav = useNavigate();
  // hook to get the query client
  const queryClient = useQueryClient();
  // React-query method for POST, PUT & DELETE is useMutation
  // Response type, Error type, Type we send to the API
  // For mutations we don't need cache so it's not entered
  return useMutation<AxiosResponse, AxiosError<Problem>, House>(
    (h) => axios.post(`${config.baseApiUrl}/houses`, h),
    {
      onSuccess: () => {
        // Refresh cache to sync db and client data
        queryClient.invalidateQueries(["houses"]);
        nav("/");
      }
    }
  )
}

const useUpdateHouse = () => {
  const nav = useNavigate();
  // hook to get the query client
  const queryClient = useQueryClient();
  // React-query method for POST, PUT & DELETE is useMutation
  // Response type, Error type, Type we send to the API
  // For mutations we don't need cache so it's not entered
  return useMutation<AxiosResponse, AxiosError<Problem>, House>(
    (h) => axios.put(`${config.baseApiUrl}/houses`, h),
    {
      onSuccess: (_, house) => {
        // Refresh cache to sync db and client data
        queryClient.invalidateQueries(["houses"]);
        nav(`/house/${house.id}`);
      }
    }
  )
}

const useDeleteHouse = () => {
  const nav = useNavigate();
  // hook to get the query client
  const queryClient = useQueryClient();
  // React-query method for POST, PUT & DELETE is useMutation
  // Response type, Error type, Type we send to the API
  // For mutations we don't need cache so it's not entered
  return useMutation<AxiosResponse, AxiosError<Problem>, House>(
    (h) => axios.delete(`${config.baseApiUrl}/houses/${h.id}`),
    {
      onSuccess: (_, house) => {
        // Refresh cache to sync db and client data
        queryClient.invalidateQueries(["houses"]);
        nav('/');
      }
    }
  )
}

export default useFetchHouses;
export { useFetchHouse, useAddHouse, useUpdateHouse, useDeleteHouse };