import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../store";
import apiClient from "../services/apiClient";

type TransportationItem = {
    id: number;
    code: string;
    fromCityName: string;
    fromCountryName: string;
    toCityName: string;
    toCountryName: string;
    departureTime: string;
    arrivalTime: string;
    seatsTotal: number;
    seatsAvailable: number;
    statusName: string;
    quantity: number;
};

export default function Transportations() {
    const user = useAppSelector((redux) => redux.auth.user);
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const { data, isLoading, error } = useQuery<TransportationItem[]>({
        queryKey: ["transportations"],
        enabled: !!user,
        queryFn: async () => {
            const res = await apiClient.get<TransportationItem[]>(`/Transportations/GetList`);
            return res.data;
        },
    });

    const addToCartMutation = useMutation({
        mutationFn: async (payload: { transportationId: number; quantity: number }) => {
            await apiClient.post(`/Cart`, payload);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["transportations"] });
        },
    });

    const handleAddToCart = (id: number) => {
        if (!user) {
            navigate("/user/login");
            return;
        }
        addToCartMutation.mutate({ transportationId: id, quantity: 1 });
    };

    if (!user) {
        return (
            <div className="p-6 text-center">
                <p className="mb-4 text-lg">Щоб переглянути рейси, увійдіть до системи.</p>
                <button
                    className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                    onClick={() => navigate("/user/login")}
                >
                    Увійти
                </button>
            </div>
        );
    }

    if (isLoading) return <p className="p-6 text-center">Downloading...</p>;
    if (error) return <p className="p-6 text-center text-red-600">Error downloading journey</p>;

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4 p-6">
            {data?.map((t) => (
                <div key={t.id} className="border p-4 rounded shadow bg-white">
                    <div className="flex items-center justify-between mb-2">
                        <h3 className="font-bold text-lg">{t.code}</h3>
                        <span className="px-3 py-1 rounded-full bg-gray-100 text-sm">{t.statusName}</span>
                    </div>
                    <p className="text-sm text-gray-700 mb-1">
                        {t.fromCityName}, {t.fromCountryName} → {t.toCityName}, {t.toCountryName}
                    </p>
                    <p className="text-sm text-gray-700 mb-1">Departure: {t.departureTime}</p>
                    <p className="text-sm text-gray-700 mb-1">Arriving: {t.arrivalTime}</p>
                    <p className="text-sm text-gray-700 mb-2">
                        Вільних місць: {t.seatsAvailable} / {t.seatsTotal}
                    </p>
                    <div className="mt-4 flex items-center gap-3">
                        <button
                            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-60"
                            onClick={() => handleAddToCart(t.id)}
                            disabled={addToCartMutation.isPending}
                        >
                            Add to Cart
                        </button>
                        <span className="text-sm">In Cart: {t.quantity}</span>
                    </div>
                </div>
            ))}
        </div>
    );
}
