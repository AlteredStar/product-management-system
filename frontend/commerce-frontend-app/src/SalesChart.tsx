import { useState, useEffect } from 'react';
import { Bar } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  
} from 'chart.js';

// Register the specific Chart.js components we intend to use
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const SalesChart = () => {
  // Store chart data formatted for react-chartjs-2
  //const [orderItems, setOrderItems] = useState<{ orderItemsId: number; orderId: number; productId: number; quantity: number; }[]>([]);
  const [chartData, setChartData] = useState({
    labels: [],
    datasets: [
      {
        label: 'Sales ($)',
        data: [],
        backgroundColor: 'rgba(75, 192, 192, 0.6)',
        borderColor: 'rgba(75, 192, 192, 1)',
        borderWidth: 1,
      },
    ],
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const createChartByCategory = async () => {
      // Implementation to create chart data by category
      try {
        const response = await fetch(`http://localhost:5045/api/orderitems/category`);

        if (!response.ok) {
          throw new Error(`Failed to fetch order items: ${response.status}`);
        }
        
        const data = await response.json();
        console.log('Raw API response:', data);

        // Handle empty data
        if (!data || data.length === 0) {
          setChartData((prevData) => ({
            ...prevData,
            labels: [],
            datasets: [
              {
                ...prevData.datasets[0], 
                data: [],
              },
            ],
          }));
          setLoading(false);
          return;
        }

        const parsedData = data.map((item: any) => ({
          category: item.categoryName || item.item1 || 'Unknown',
          revenue: parseFloat(item.totalRevenue || item.item2 || 0)
        }));
        console.log(parsedData);
        
        setChartData((prevData) => ({
          ...prevData,
          labels: parsedData.map((item: any) => item.category),
          datasets: [
            {
              ...prevData.datasets[0], 
              data: parsedData.map((item: any) => item.revenue),
            },
          ],
        }));

        setLoading(false);
      } catch (err) {
        console.error('Error fetching chart data:', err);
        setError((err as Error));
        setLoading(false);
      }
    };

    createChartByCategory();
  }, []);

  const options = {
    responsive: true,
    plugins: {
      legend: { position: 'top' as const },
      title: { display: true, text: 'Sales by Product Categories' },
    },
  };

  if (loading) return <p>Loading data from database...</p>;
  if (error) return <p>Error loading chart: {error.message}</p>;

  return (
    <div style={{ width: '80%', margin: '0 auto' }}>
      {chartData && <Bar data={chartData} options={options} />}
    </div>
  );
};

export default SalesChart;
