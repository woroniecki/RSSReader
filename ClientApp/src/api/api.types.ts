interface User {
  userName: string
  email: string
}

export interface LoginRequest {
  username: string
  password: string
}
export interface LoginResponse {
  token: string
  expiration: number
  user: User
}
