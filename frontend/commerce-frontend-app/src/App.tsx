import { useState } from 'react'
// import reactLogo from './assets/react.svg'
// import viteLogo from './assets/vite.svg'
// import heroImg from './assets/hero.png'
import './App.css'
//import axios from 'axios';
import DropdownWithApi from './ProdCategoryDropDown.tsx'; 

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <section id="center">
        <div>
          <h1>Welcome to Commerce Manager</h1>
        </div>
      </section>

      <div className="ticks"></div>

      <section id="next-steps">
        <div id="docs">
          <h2>Add Product</h2>
          <p>Enter product details below to add a new product.</p>
          
          <form action="/submit-path" method="POST">
            <label htmlFor="product-name">Product Name:</label>
            <input type="text" id="product-name" name="product-name" required/>

            <label htmlFor="product-description">Product Description:</label>
            <input type="text" id="product-description" name="product-description" required/>

            <DropdownWithApi />

            <label htmlFor="product-price">Product Price:</label>
            <input type="number" id="product-price" name="product-price" required/>

            <button type="submit">Submit</button>
          </form>

        </div>
      </section>

      <div className="ticks"></div>
      <section id="spacer"></section>

      <section id="next-steps">
        <div id="social">
          <p>Connect</p>
          <ul>
            <li>
              <a href="https://github.com/vitejs/vite" target="_blank">
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
              <a href="https://chat.vite.dev/" target="_blank">
                <svg
                  className="button-icon"
                  role="presentation"
                  aria-hidden="true"
                >
                  <use href="/icons.svg#discord-icon"></use>
                </svg>
                Discord
              </a>
            </li>
          </ul>
        </div>
      </section>
    </>
  )
}

export default App
