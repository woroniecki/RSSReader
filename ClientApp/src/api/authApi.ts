import axios from 'api/axios'
import env from 'react-dotenv'
import { LoginRequest, LoginResponse } from './api.types'

export const register = async (data: any) => {
  try {
    const res = await axios.post(`/api/auth/register`, data)
  } catch (error) {
    throw error.response
  }
}

export const login = async (data: LoginRequest) => {
  try {
    const res = await axios.post(`/api/auth/login`, data)
    return res.data.result as LoginResponse
  } catch (error) {
    throw error.response
  }
}
