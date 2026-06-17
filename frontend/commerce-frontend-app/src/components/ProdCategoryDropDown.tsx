import { useState, useEffect } from 'react';
import './AddProduct.css'

function ProductCategoryDropDown() {
  const [data, setData] = useState<{ categoryId: number; name: string; description: string }[]>([]); 
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5045/api/productcategory')
      .then((response) => {
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        return response.json();
      })
      .then((data) => {
        setData(data);
        setLoading(false);
      })
      .catch((error) => {
        setError(error);
        setLoading(false);
      });
  }, []);

  if (loading) return <div>Loading options...</div>;
  if (error) return <div>Error loading data: {(error as Error).message}</div>;

  return (
    <>
      <label htmlFor="database-dropdown">Choose an item:</label>
      <select id="database-dropdown" name="categoryId" defaultValue="">
        <option value="" disabled>
          -- Select an option --
        </option>
        {data.map((item) => (
          <option key={item.categoryId} value={item.categoryId}>
            {item.name}
          </option>
        ))}
      </select>
    </>
  );
}

export default ProductCategoryDropDown;
