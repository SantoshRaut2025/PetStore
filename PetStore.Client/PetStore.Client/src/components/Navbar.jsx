import './Navbar.css'

const Navbar = ({ username = 'Guest', onLogout }) => {
  return (
    <header className="navbar">
      <div className="navbar__brand">
        <span className="navbar__logo">PetStore</span>
      </div>

      <nav className="navbar__menu" aria-label="Main navigation">
        <a className="navbar__link" href="#">
          Products
        </a>
        <a className="navbar__link" href="#">
          Cart
        </a>
        <a className="navbar__link" href="#">
          User
        </a>
      </nav>

      <div className="navbar__actions">
        <button className="navbar__action-btn navbar__cart" type="button">
          Cart
        </button>
        <div className="navbar__user-chip" title="Signed in user">
          <span className="navbar__user-name">{username}</span>
        </div>
        {onLogout && (
          <button className="navbar__action-btn navbar__logout" onClick={onLogout} type="button">
            Logout
          </button>
        )}
      </div>
    </header>
  )
}

export default Navbar

