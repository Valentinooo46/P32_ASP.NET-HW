import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../store";
import apiClient from "../services/apiClient";

type CartItem = {
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

const CartPage = () => {
  const user = useAppSelector((redux) => redux.auth.user);
  const navigate = useNavigate();

  const { data, isLoading, error } = useQuery<CartItem[]>({
    queryKey: ["cart"],
    enabled: !!user,
    queryFn: async () => {
      const res = await apiClient.get<CartItem[]>("/Cart");
      return res.data;
    },
  });

  if (!user) {
    return (
      <div className="p-6 text-center">
        <p className="mb-4 text-lg">Щоб побачити кошик, увійдіть до системи.</p>
        <button
          className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          onClick={() => navigate("/user/login")}
        >
          Увійти
        </button>
      </div>
    );
  }

  if (isLoading) return <p className="p-6 text-center">Завантаження кошика...</p>;
  if (error) return <p className="p-6 text-center text-red-600">Помилка завантаження кошика</p>;

  const totalTickets = data?.reduce((sum, item) => sum + item.quantity, 0) ?? 0;

  return (
    <div className="p-6 space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold">Мої замовлення</h2>
        <div className="text-sm text-gray-700">Всього квитків: {totalTickets}</div>
      </div>

      {(!data || data.length === 0) && (
        <div className="text-center text-gray-600">Кошик порожній</div>
      )}

      <div className="space-y-3">
        {data?.map((item) => (
          <div key={item.id} className="border rounded-lg p-4 bg-white shadow-sm">
            <div className="flex items-center justify-between mb-1">
              <div className="font-semibold text-lg">{item.code}</div>
              <span className="px-2 py-1 text-xs bg-gray-100 rounded-full">{item.statusName}</span>
            </div>
            <p className="text-sm text-gray-700">
              {item.fromCityName}, {item.fromCountryName} ? {item.toCityName}, {item.toCountryName}
            </p>
            <p className="text-sm text-gray-700">Виїзд: {item.departureTime}</p>
            <p className="text-sm text-gray-700">Прибуття: {item.arrivalTime}</p>
            <div className="mt-2 flex items-center justify-between text-sm">
              <span>Місць доступно: {item.seatsAvailable} / {item.seatsTotal}</span>
              <span className="font-semibold">Замовлено: {item.quantity}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default CartPage;
