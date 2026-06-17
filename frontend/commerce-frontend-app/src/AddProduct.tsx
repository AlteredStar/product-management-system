import ProdCategoryDropDown from "./ProdCategoryDropDown";
import { type SubmitEvent } from 'react';
import './AddProduct.css'
import './App.css';

function AddProduct() {
  const handleSubmit = async (e: SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();

    const form = e.currentTarget as HTMLFormElement;
    const fd = new FormData(form);

    const payload = {
      name: (fd.get('name') as string) || '',
      description: (fd.get('description') as string) || '',
      categoryId: Number(fd.get('categoryId') || 0),
      price: Number(fd.get('price') || 0),
      stock: Number(fd.get('stock') || 0)
    };

    try {
      const res = await fetch('http://localhost:5045/api/products', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || 'Failed to create product');
      }

      alert('Product Created Successfully');
      form.reset();
    } catch (err) {
      alert(`Error creating product: ${(err as Error).message}`);
    }
  };

  return (
    <div>
      <form onSubmit={handleSubmit} className="form-container">
        <div className="form-group">
        <label htmlFor="product-name">Product Name:</label>
        <input type="text" id="product-name" name="name" required/>
        </div>

        <div className="form-group">
        <label htmlFor="product-description">Product Description:</label>
        <input type="text" id="product-description" name="description" required/>
        </div>

        <div className="form-group">
        <ProdCategoryDropDown />
        </div>

        <div className="form-group">
        <label htmlFor="product-price">Product Price:</label>
        <input type="number" id="product-price" name="price" required/>
        </div>

        <div className="form-group">
        <label htmlFor="product-stock">Product Stock:</label>
        <input type="number" id="product-stock" name="stock" required/>
        </div>

        <button type="submit" className="buttons">Submit</button>
      </form>
    </div>
  );
}

export default AddProduct;
