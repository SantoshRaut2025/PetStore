import { useState } from 'react'
import Login from './components/Login'
import Navbar from './components/Navbar'
import dogHero from './assets/golder-retriever-puppy.webp'
import './App.css'

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [user, setUser] = useState(null)
  const [currentSlide, setCurrentSlide] = useState(0)

  const slides = [
    {
      id: 'dogs',
      image: dogHero,
      alt: 'Golden Retriever',
      title: 'For Dogs',
      description: 'Food, toys, and essentials for every good boy and girl.',
    },
    {
      id: 'cats',
      image: 'https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=1400&q=80',
      alt: 'Curious cat',
      title: 'For Cats',
      description: 'Cozy beds, premium treats, and playful accessories.',
    },
  ]

  const handleLoginSuccess = (loginData) => {
    setUser(loginData)
    setIsAuthenticated(true)
  }

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('username')
    localStorage.removeItem('expiresIn')
    setUser(null)
    setIsAuthenticated(false)
  }

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % slides.length)
  }

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length)
  }

  return (
    <>
      {!isAuthenticated ? (
        <Login onLoginSuccess={handleLoginSuccess} />
      ) : (
        <div className="app-shell">
          <Navbar username={user?.username} onLogout={handleLogout} />
          <main className="app-content">
            <h1>Pet Store</h1>
            <p>Welcome, {user?.username}!</p>
            <section className="carousel">
              <button className="carousel__nav carousel__nav--prev" onClick={prevSlide} aria-label="Previous">
                ‹
              </button>
              <div className="carousel__viewport">
                <div className="carousel__track" style={{ transform: `translateX(-${currentSlide * 100}%)` }}>
                  {slides.map((slide) => (
                    <div className="carousel__slide" key={slide.id}>
                      <div className="hero-card">
                        <img src={slide.image} alt={slide.alt} loading="lazy" />
                        <div className="hero-card__body">
                          <h3>{slide.title}</h3>
                          <p>{slide.description}</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
              <button className="carousel__nav carousel__nav--next" onClick={nextSlide} aria-label="Next">
                ›
              </button>
            </section>
            <div className="carousel__dots" role="tablist" aria-label="Pet categories">
              {slides.map((slide, index) => (
                <button
                  key={slide.id}
                  className={`carousel__dot ${index === currentSlide ? 'is-active' : ''}`}
                  aria-label={`Show ${slide.title}`}
                  aria-pressed={index === currentSlide}
                  onClick={() => setCurrentSlide(index)}
                  type="button"
                />
              ))}
            </div>
          </main>
        </div>
      )}
    </>
  )
}

export default App
