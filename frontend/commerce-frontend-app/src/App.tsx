import './App.css'
import AddProduct from './AddProduct.tsx';
import ManageProduct from './ManageProduct.tsx';

function App() {
  return (
    <>
      <section id="center">
        <div>
          <h1>Welcome to Commerce Manager</h1>
        </div>
      </section>

      <div className="ticks"></div>

      <section id="actions">
        <div id="docs">
          <h2>Add a Product</h2>
          <p>Enter product details below to add a new product.</p>
          <AddProduct />
        </div>
      </section>

      <div className="ticks"></div>

      <section id="actions">
        <div id="docs">
          <h2>Manage Products</h2>
          <p>Search for, edit, and delete existing products.</p>
          <ManageProduct />
        </div>
      </section>

      <div className="ticks"></div>
      <section id="spacer"></section>

      <section id="footer">
        <div id="social">
          <p>Connect</p>
          <ul>
            <li>
              <a href="https://github.com/AlteredStar/product-management-system" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#github-icon"></use>
                </svg>
                GitHub
              </a>
            </li>
            <li>
              <a href="https://www.linkedin.com/in/kevin-dean-nguyen" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#linkedin-icon"></use>
                </svg>
                LinkedIn
              </a>
            </li>
          </ul>
        </div>
      </section>
    </>
  )
}

export default App
