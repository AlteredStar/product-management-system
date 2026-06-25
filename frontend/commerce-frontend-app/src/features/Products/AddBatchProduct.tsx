import { useState, type ChangeEvent } from 'react';
import '../../App.css';
import './AddProduct.css'

function AddBatchproduct() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState('');

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];

    if (file) {
      setFile(file);
    }
  };

  const handleSubmit = async () => {
    if (!file) {
      setStatus('Please select a CSV file first.');
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      setStatus('Uploading...');
      const response = await fetch('https://localhost:5001/api/products/batch', {
        method: 'POST',
        body: formData,
      });

      if (response.ok) {
        setStatus('Upload successful!');
      } else {
        const errorText = await response.text();
        setStatus(`Upload failed: ${errorText}`);
      }
    } catch (err) {
      setStatus('Network error occurred.');
    }
  };


  return (
    <div className="add-product">
      <form onSubmit={handleSubmit}>
        <input type="file" accept=".csv" className="buttons" onChange={handleFileChange} />
        <button type="submit" className="buttons">Upload CSV</button>
      </form>
      <p>{status}</p>
    </div>
  );
}

export default AddBatchproduct;