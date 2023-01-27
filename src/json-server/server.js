// const jsonServer = require("json-server");
// const server = jsonServer.create();
// const router = jsonServer.router("./data/data.json");
// const middlewares = jsonServer.defaults();

// server.use(middlewares);
// server.use(router);
// server.use((req, res, next) => {
//   res.header("Access-Control-Allow-Origin", "http://localhost:3000");
//   res.header("Access-Control-Allow-Headers", "*");
//   next();
// });
// server.listen(3000, () => {
//   console.log("JSON Server is running");
// });

// module.exports = server;

// module.exports = (req, res, next) => {
//   res.header("Access-Control-Allow-Origin", "http://localhost:3000");
//   res.header("Access-Control-Allow-Headers", "*");
//   next();
// };

const jsonServer = require("json-server");
const clone = require("clone");
const data = require("./data/data.json");

const server = jsonServer.create();
const router = jsonServer.router("./data/data.json");
const middlewares = jsonServer.defaults();

server.use(middlewares);
server.use((req, res, next) => {
  if (req.path !== "/") router.db.setState(clone(data));
  next();
});

server.use(router);
server.listen(3000, () => {
  console.log("JSON Server is running");
});

// Export the Server API
module.exports = server;
