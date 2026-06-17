import { useState, useEffect } from 'react';
import { Line, Bar } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  
} from 'chart.js';
import './SalesChart.css'
import './App.css'

ChartJS.register(CategoryScale, LinearScale, BarElement, PointElement, LineElement, Title, Tooltip, Legend);

interface ChartData {
  labels: string[];
  datasets: {
    label: string;
    data: any[];
    backgroundColor: string;
    borderColor: string;
    borderWidth: number;
  }[];
}

const SalesChart = () => {
  const [chartData, setChartData] = useState<ChartData>({
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
 
  const [activeTab, setActiveTab] = useState("displayByDate");
  const [chartType, setChartType] = useState("line");

  useEffect(() => {
    const createChartByDate = async () => {
      try {
        const response = await fetch(`http://localhost:5045/api/orderitems/date`);

        if (!response.ok) {
          throw new Error(`Failed to fetch order items: ${response.status}`);
        }
        
        const data = await response.json();

        const parsedData = data.map((item: any) => ({
          year: (new Date(item.orderDate)).getFullYear(),
          month: (new Date(item.orderDate)).getMonth(),
          revenue: parseFloat(item.totalRevenue)
        }));
        
        const targetYear = 2025; // TODO: add dynamic year selection 

        const monthLabels: string[] = Array.from({ length: 12 }, (_, i) => {
          return new Date(0, i).toLocaleString('en-US', { month: 'long' });
        });
        const aggregatedData: number[] = new Array(12).fill(0);
        
        const filteredData = parsedData.filter((item: any) => item.year === targetYear);

        filteredData.forEach((curr: any) => {
          aggregatedData[curr.month] += curr.revenue;
        });

        setChartData((prevData) => ({
          ...prevData,
          labels: monthLabels,
          datasets: [
            {
              ...prevData.datasets[0],
              data: aggregatedData,
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

    const createChartByCategory = async () => {
      try {
        const response = await fetch(`http://localhost:5045/api/orderitems/category`);

        if (!response.ok) {
          throw new Error(`Failed to fetch order items: ${response.status}`);
        }
        
        const data = await response.json();

        const parsedData = data.map((item: any) => ({
          category: item.categoryName,
          revenue: parseFloat(item.totalRevenue)
        }));
        
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

    if (activeTab === "displayByDate") {
      setChartType("line");
      createChartByDate();
    }
    else if (activeTab === "displayByCategory") {
      setChartType("bar");
      createChartByCategory();
    }
  }, [activeTab]);

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
    <div>
      <div className="toggle-wrapper">
        <div className="toggle-container">
          <button
            className={`toggle-btn ${activeTab === "displayByDate" ? "active" : ""}`}
            onClick={() => setActiveTab("displayByDate")}
          >
            Display by Date
          </button>
          <button
            className={`toggle-btn ${activeTab === "displayByCategory" ? "active" : ""}`}
            onClick={() => setActiveTab("displayByCategory")}
          >
            Display by Category
          </button>
        </div>
      </div>
    
      <div style={{ width: '80%', margin: '0 auto' }}>
        {chartData && chartType === "line" && <Line data={chartData} options={options} redraw={true} />}
        {chartData && chartType === "bar" && <Bar data={chartData} options={options} redraw={true} />}
      </div>
    </div>
  );
};

export default SalesChart;
