export const API_URL = (): string => {
  if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
    return process.env.REACT_APP_PRODUCTION_API_URL // will return API URL in .env file.
  } else {
    return process.env.REACT_APP_DEVELPOMENT_API_URL
  }
}

export const GET_NODE_ENV = (): string => {
  return process.env.NODE_ENV
}
