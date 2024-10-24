import "../styles/defaultheader.css"

export function Defaultheather() {
  return (
    <div className='defaultHeader'>
      
      <ul>
          <div>
            <li className='signin_button pointer'><a href="#">sign in</a></li>
          </div>
          <div>
            <li className='signup_button pointer'><a href="#">sign up</a></li>
          </div>
      </ul>
    </div>
  )
}