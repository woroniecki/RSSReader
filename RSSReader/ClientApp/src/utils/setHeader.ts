import axios from 'api/axios'

export const setAuthHeader = (token: string): void => {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
}

export const removeAuthHeader = (): void => {
  delete axios.defaults.headers.common['Authorization']
}
