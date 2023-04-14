import {
  useTable,
  usePagination,
  useFilters,
  useGlobalFilter,
} from "react-table";
import {
  Box,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  TableContainer,
  Flex,
  IconButton,
  Text,
  Tooltip,
  Select,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  NumberIncrementStepper,
  NumberDecrementStepper,
} from "@chakra-ui/react";
import {
  ArrowRightIcon,
  ArrowLeftIcon,
  ChevronRightIcon,
  ChevronLeftIcon,
} from "@chakra-ui/icons";
import { GlobalFilter, DefaultFilterForColumn } from "components";

/**
 * The following code is based on these sources:
 * https://codesandbox.io/s/react-table-chakra-ui-pagination-example-forked-e9y9he?file=/src/App.js
 * https://stackoverflow.com/questions/64608974/react-table-pagination-properties-doesnt-exist-on-type-tableinstance
 * https://www.bacancytechnology.com/blog/react-table-tutorial
 * https://github.com/TanStack/table/issues/1825
 * https://tanstack.com/table/v8/docs/api/features/filters?from=reactTableV7&original=https://react-table-v7.tanstack.com/docs/api/useFilters
 */
function PaginatedTable(props: { columns: any[]; data: any[] }) {
  const data = props.data;
  const columns = props.columns;

  const {
    getTableProps,
    getTableBodyProps,
    headerGroups,
    prepareRow,
    page,
    canPreviousPage,
    canNextPage,
    pageOptions,
    pageCount,
    gotoPage,
    nextPage,
    previousPage,
    setPageSize,
    state: { pageIndex, pageSize, globalFilter },
    setGlobalFilter,
    preGlobalFilteredRows,
  } = useTable(
    {
      columns,
      autoResetFilters: false,
      autoResetGlobalFilter: false,
      defaultColumn: { Filter: DefaultFilterForColumn },
      data,
      initialState: { pageIndex: 0, pageSize: 25 },
      // Table will always reset to page 1 when new data is loaded, unless autoResetPage is false.
      autoResetPage: false,
    },
    useFilters,
    useGlobalFilter,
    usePagination
  );

  return (
    <>
      <TableContainer mt={"50px"}>
        <Table variant={"striped"} colorScheme={"gray"} {...getTableProps()}>
          <Thead>
            {headerGroups.map((headerGroup) => (
              <Tr {...headerGroup.getHeaderGroupProps()}>
                {headerGroup.headers.map((column: any) => (
                  <Th {...column.getHeaderProps()}>
                    {headerGroup.headers.length == 1 ? (
                      <Box onFocus={() => gotoPage(0)}>
                        <GlobalFilter
                          preGlobalFilteredRows={preGlobalFilteredRows}
                          globalFilter={globalFilter}
                          setGlobalFilter={setGlobalFilter}
                        />
                      </Box>
                    ) : (
                      column.render("Header")
                    )}
                    {column.canFilter ? (
                      <Box onFocus={() => gotoPage(0)}>
                        {column.render("Filter")}
                      </Box>
                    ) : null}
                  </Th>
                ))}
              </Tr>
            ))}
          </Thead>
          <Tbody {...getTableBodyProps()}>
            {page.map((row: any) => {
              prepareRow(row);
              return (
                <Tr {...row.getRowProps()}>
                  {row.cells.map((cell: any) => {
                    return (
                      <Td {...cell.getCellProps()}>{cell.render("Cell")}</Td>
                    );
                  })}
                </Tr>
              );
            })}
          </Tbody>
        </Table>
      </TableContainer>

      <Flex justifyContent="space-between" m={4} alignItems="center">
        <Flex>
          <Tooltip label="First Page">
            <IconButton
              aria-label={"First Page"}
              onClick={() => gotoPage(0)}
              isDisabled={!canPreviousPage}
              icon={<ArrowLeftIcon h={3} w={3} />}
              mr={4}
            />
          </Tooltip>
          <Tooltip label="Previous Page">
            <IconButton
              aria-label={"Previous Page"}
              onClick={previousPage}
              isDisabled={!canPreviousPage}
              icon={<ChevronLeftIcon h={6} w={6} />}
            />
          </Tooltip>
        </Flex>

        <Flex alignItems="center">
          <Text flexShrink="0" mr={8}>
            Page{" "}
            <Text fontWeight="bold" as="span">
              {pageIndex + 1}
            </Text>{" "}
            of{" "}
            <Text fontWeight="bold" as="span">
              {pageOptions.length}
            </Text>
          </Text>
          <Text flexShrink="0">Go to page:</Text>{" "}
          <NumberInput
            ml={2}
            mr={8}
            w={28}
            min={1}
            max={pageOptions.length}
            onChange={(value) => {
              const page = value ? parseInt(value) - 1 : 0;
              gotoPage(page);
            }}
            defaultValue={pageIndex + 1}
            aria-label={"Current Table Page"}
          >
            <NumberInputField />
            <NumberInputStepper>
              <NumberIncrementStepper />
              <NumberDecrementStepper />
            </NumberInputStepper>
          </NumberInput>
          <Select
            w={32}
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
            }}
            aria-label={"Visible Table Rows"}
          >
            {[25, 50, 75, 100].map((pageSize) => (
              <option key={pageSize} value={pageSize}>
                Show {pageSize}
              </option>
            ))}
          </Select>
        </Flex>

        <Flex>
          <Tooltip label="Next Page">
            <IconButton
              aria-label={"Next Page"}
              onClick={nextPage}
              isDisabled={!canNextPage}
              icon={<ChevronRightIcon h={6} w={6} />}
            />
          </Tooltip>
          <Tooltip label="Last Page">
            <IconButton
              aria-label={"Last Page"}
              onClick={() => gotoPage(pageCount - 1)}
              isDisabled={!canNextPage}
              icon={<ArrowRightIcon h={3} w={3} />}
              ml={4}
            />
          </Tooltip>
        </Flex>
      </Flex>
    </>
  );
}

export { PaginatedTable };
