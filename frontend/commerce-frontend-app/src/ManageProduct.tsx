import { useState, useEffect, type ChangeEvent, type SubmitEvent } from 'react';
import './App.css'

interface Product {
  productId: number;
  categoryId: number;
  categoryName?: string;
  name: string;
  description: string;
  price: number;
  stock: number;
}

interface ProductCategory {
  categoryId: number;
  name: string;
}

interface PaginatedResponse {
  rows: Product[];
  totalPages: number;
}

function ManageProduct() {
  const [data, setData] = useState<Product[]>([]);
  const [searchText, setSearchText] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [editRowId, setEditRowId] = useState<number | null>(null);
  
  const [editedData, setEditedData] = useState<Product>({
    productId: 0,
    categoryId: 0,
    name: '',
    description: '',
    price: 0,
    stock: 0
  });
  
  const [category, setCategory] = useState<ProductCategory[]>([]);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1); 
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    fetch(`http://localhost:5045/api/productcategory`)
      .then(res => res.json())
      .then((categories: ProductCategory[]) => {
        setCategory(categories);
        setLoading(false);
      });
  }, []);

  const fetchProducts = (query: string, page: number) => {
    setLoading(true);
    fetch(`http://localhost:5045/api/products?page=${page}&pageSize=10&search=${encodeURIComponent(query)}`)
      .then(res => {
        if (!res.ok) throw new Error('Network response error');
        return res.json();
      })
      .then((result: PaginatedResponse) => {
        setData(result?.rows || []);
        setTotalPages(result?.totalPages || 1);
        setLoading(false);
      })
      .catch(err => {
        console.error("Error fetching data:", err);
        setData([]);
        setLoading(false);
      });
  };

  useEffect(() => {
    fetchProducts(searchQuery, currentPage);
  }, [currentPage, searchQuery]);

  const handleSearchChange = (e: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(e.target.value);
  };

  const handleSearchSubmit = (e: SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    setCurrentPage(1);
    setSearchQuery(searchText);
    fetchProducts(searchText, 1);
  };

  const handleEditClick = (row: Product): void => {
    setEditRowId(row.productId);
    setEditedData({ ...row });
  };

  const handleDeleteClick = (row: Product): void => {
    if (window.confirm(`Are you sure you want to delete the product with Product ID: ${row.productId}?`)) {
      fetch(`http://localhost:5045/api/products/${row.productId}`, {
        method: 'DELETE',
      })
        .then(res => {
          if (!res.ok) throw new Error('Failed to delete record');
          setData(data.filter(item => item.productId !== row.productId));
        })
        .catch(err => alert(err.message));
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
    const { name, value, type } = e.target;
    
    setEditedData(prev => ({
      ...prev,
      [name]: type === 'number' ? Number(value) : value
    }));
  };

  const handleSave = (id: number): void => {
    fetch(`http://localhost:5045/api/products/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(editedData),
    })
      .then(res => {
        if (!res.ok) throw new Error('Failed to update record');
        setData(data.map(row => (row.productId === id ? editedData : row)));
        setEditRowId(null);
      })
      .catch(err => alert(err.message));
  };

  return (
    <div>
      <form onSubmit={handleSearchSubmit} style={{ marginBottom: '10px' }}>
        <input
          type="text"
          placeholder="Search products by name..."
          value={searchText}
          onChange={handleSearchChange}
          style={{ marginRight: '8px' }}
        />
        <button type="submit" className="buttons">Search</button>
      </form>

      {loading && data.length === 0 && <div>Loading records...</div>}

      <table border={1} style={{ width: '100%', marginTop: '10px', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Category</th>
            <th>Description</th>
            <th>Price</th>
            <th>Stock</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data?.map((row: Product) => (
            <tr key={row.productId}>
              {editRowId === row.productId ? (
                <>
                  <td><input name="name" value={editedData.name} onChange={handleInputChange} /></td>
                  <td><input name="category" value={category.find(c => c.categoryId === editedData.categoryId)?.name} onChange={handleInputChange} /></td>
                  <td><input name="description" value={editedData.description} onChange={handleInputChange} /></td>
                  <td><input type="number" name="price" value={editedData.price} onChange={handleInputChange} /></td>
                  <td><input type="number" name="stock" value={editedData.stock} onChange={handleInputChange} /></td>
                  <td>
                    <button onClick={() => handleSave(row.productId)} className="buttons">Save</button>
                    <button onClick={() => setEditRowId(null)} className="buttons">Cancel</button>
                  </td>
                </>
              ) : (
                <>
                  <td>{row.name}</td>
                  <td>{category.find(c => c.categoryId === row.categoryId)?.name}</td>
                  <td>{row.description}</td>
                  <td>${(Number(row.price) || 0).toFixed(2)}</td>
                  <td>{row.stock}</td>
                  <td>
                    <button onClick={() => handleEditClick(row)} className="buttons">Edit</button>
                    <button onClick={() => handleDeleteClick(row)} className="buttons">Delete</button>
                  </td>
                </>
              )}
            </tr>
          ))}
        </tbody>
      </table>

      {data.length > 0 && (
        <div style={{ marginTop: '10px' }}>
          <button disabled={currentPage === 1} onClick={() => setCurrentPage(prev => prev - 1)} className="buttons">
            Previous
          </button>
          <span> Page {currentPage} of {totalPages || 1} </span>
          <button disabled={currentPage === totalPages || totalPages === 0} onClick={() => setCurrentPage(prev => prev + 1)}>
            Next
          </button>
        </div>
      )}
    </div>
  );

}

export default ManageProduct;
