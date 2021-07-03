import axios from 'api/axios'
import { LoginRequest, LoginResponse, RefreshRequest } from './api.types'

export const register = async (data: any): Promise<void> => {
  try {
    await axios.post(`/api/auth/register`, data)
  } catch (error) {
    throw error.response
  }
}

export const login = async (data: LoginRequest): Promise<LoginResponse> => {
  try {
    const res = await axios.post(`/api/auth/login`, data)
    return res.data.result as LoginResponse
  } catch (error) {
    throw error.response
  }
}

export const refresh = async (data: RefreshRequest): Promise<LoginResponse> => {
  try {
    const res = await axios.post(`/api/auth/refresh`, data)
    return res.data.result as LoginResponse
  } catch (error) {
    throw error.response
  }
}
